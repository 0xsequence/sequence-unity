using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.ABI;
using Sequence.Mocks;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Wallet;
using System.Collections.Generic;
using Sequence.Contracts;
using Sequence.Utils;

namespace Sequence.Ethereum.Tests
{
    public class SequenceEthClientTests
    {
        static readonly string testnetUrl = "http://localhost:8545/";
        static readonly string publicPolygonRpc = "https://polygon-bor.publicnode.com";
        private static string[] urls = new string[] { testnetUrl, publicPolygonRpc };
        float blockTimeInSeconds = 2f;
        IRpcClient failingClient = new FailingRpcClient();
        const string validAddress = "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C";
        EthWallet wallet1 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        EthWallet wallet2 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");

        [Test]
        public async Task TestBalanceAt()
        {
            try
            {
                BigInteger startingBalance = 10000 * BigInteger.Pow(10, 18);
                var client = new SequenceEthClient(testnetUrl);
                BigInteger balance = await client.BalanceAt(wallet1.GetAddress(), "earliest");
                Assert.AreEqual(startingBalance, balance);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestCaseSource(nameof(urls))]
        public async Task TestBlockByNumber(string url)
        {
            try
            {
                Block startingBlock = new Block()
                {
                    number = "0x0",
                    nonce = "0x0000000000000000",
                    gasUsed = "0x0",
                };
                var client = new SequenceEthClient(url);

                Block block = await client.BlockByNumber("earliest");

                Assert.AreEqual(startingBlock.number, block.number);
                Assert.AreEqual(startingBlock.nonce, block.nonce);
                Assert.AreEqual(startingBlock.gasUsed, block.gasUsed);

                await TestBlockByHash(block, url);

                block = await client.BlockByNumber("latest");

                await TestBlockByHash(block, url);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        public async Task TestBlockByHash(Block expectedBlock, string url)
        {
            try
            {
                var client = new SequenceEthClient(url);
                Block block = await client.BlockByHash(expectedBlock.hash);
                Assert.AreEqual(expectedBlock.ToString(), block.ToString());
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestCaseSource(nameof(urls))]
        public async Task TestBlockNumber(string url)
        {
            try
            {
                var client = new SequenceEthClient(url);
                var blockNumber = await client.BlockNumber();
                await Task.Delay((int)(blockTimeInSeconds * 1000 * 3)); // Wait for more than the block time just in case
                var blockNumber2 = await client.BlockNumber();
                Assert.Less(blockNumber, blockNumber2);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestBlockRange()
        {
            try
            {
                var client = new SequenceEthClient(testnetUrl);
                
                string blockCount = "0x11";
                string blockNumber = await client.BlockNumber();
                // Must have at least blockCount blocks of fee history
                while (blockNumber.HexStringToBigInteger() < blockCount.HexStringToBigInteger()) {
                    await Task.Delay((int)(blockTimeInSeconds * 1000));
                    blockNumber = await client.BlockNumber();
                }

                List<Block> blockRange = await client.BlockRange("0x0", blockCount);
                Assert.Greater(blockRange.Count, 1);

                Block startingBlock = await client.BlockByNumber("earliest");
                Assert.AreEqual(startingBlock.ToString(), blockRange[0].ToString());
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        private static List<Chain> chainIdCases = EnumExtensions.GetEnumValuesAsList<Chain>();
        
        [TestCaseSource(nameof(chainIdCases))]
        public async Task TestChainId(Chain chain)
        {
            if (chain == Chain.None) return;
            try
            {
                var client = new SequenceEthClient(chain);
                string chainId = await client.ChainID();
                Assert.AreEqual(chain.AsHexString(), chainId);
            }catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestCodeAt()
        {
            try
            {
                var client = new SequenceEthClient(testnetUrl);
                var bytecode = ERC20Tests.bytecode;
                ContractDeploymentResult result = await ContractDeployer.Deploy(client, wallet1, bytecode);
                TransactionReceipt receipt = result.Receipt;
                var contractAddress = receipt.contractAddress;
                // expectedDeployedCode is the deployedBytecode from the compiled test ERC20 smart contract
                var expectedDeployedCode = "0x608060405234801561001057600080fd5b506004361061010b5760003560e01c806370a08231116100a257806395d89b411161007157806395d89b41146102a6578063a457c2d7146102c4578063a9059cbb146102f4578063dd62ed3e14610324578063f2fde38b146103545761010b565b806370a0823114610232578063715018a61461026257806379cc67901461026c5780638da5cb5b146102885761010b565b8063313ce567116100de578063313ce567146101ac57806339509351146101ca57806340c10f19146101fa57806342966c68146102165761010b565b806306fdde0314610110578063095ea7b31461012e57806318160ddd1461015e57806323b872dd1461017c575b600080fd5b610118610370565b6040516101259190611178565b60405180910390f35b61014860048036038101906101439190611233565b610402565b604051610155919061128e565b60405180910390f35b610166610425565b60405161017391906112b8565b60405180910390f35b610196600480360381019061019191906112d3565b61042f565b6040516101a3919061128e565b60405180910390f35b6101b461045e565b6040516101c19190611342565b60405180910390f35b6101e460048036038101906101df9190611233565b610467565b6040516101f1919061128e565b60405180910390f35b610214600480360381019061020f9190611233565b61049e565b005b610230600480360381019061022b919061135d565b6104b4565b005b61024c6004803603810190610247919061138a565b6104c8565b60405161025991906112b8565b60405180910390f35b61026a610510565b005b61028660048036038101906102819190611233565b610524565b005b610290610544565b60405161029d91906113c6565b60405180910390f35b6102ae61056e565b6040516102bb9190611178565b60405180910390f35b6102de60048036038101906102d99190611233565b610600565b6040516102eb919061128e565b60405180910390f35b61030e60048036038101906103099190611233565b610677565b60405161031b919061128e565b60405180910390f35b61033e600480360381019061033991906113e1565b61069a565b60405161034b91906112b8565b60405180910390f35b61036e6004803603810190610369919061138a565b610721565b005b60606003805461037f90611450565b80601f01602080910402602001604051908101604052809291908181526020018280546103ab90611450565b80156103f85780601f106103cd576101008083540402835291602001916103f8565b820191906000526020600020905b8154815290600101906020018083116103db57829003601f168201915b5050505050905090565b60008061040d6107a4565b905061041a8185856107ac565b600191505092915050565b6000600254905090565b60008061043a6107a4565b9050610447858285610975565b610452858585610a01565b60019150509392505050565b60006012905090565b6000806104726107a4565b9050610493818585610484858961069a565b61048e91906114b0565b6107ac565b600191505092915050565b6104a6610c77565b6104b08282610cf5565b5050565b6104c56104bf6107a4565b82610e4b565b50565b60008060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050919050565b610518610c77565b6105226000611018565b565b610536826105306107a4565b83610975565b6105408282610e4b565b5050565b6000600560009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b60606004805461057d90611450565b80601f01602080910402602001604051908101604052809291908181526020018280546105a990611450565b80156105f65780601f106105cb576101008083540402835291602001916105f6565b820191906000526020600020905b8154815290600101906020018083116105d957829003601f168201915b5050505050905090565b60008061060b6107a4565b90506000610619828661069a565b90508381101561065e576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161065590611556565b60405180910390fd5b61066b82868684036107ac565b60019250505092915050565b6000806106826107a4565b905061068f818585610a01565b600191505092915050565b6000600160008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905092915050565b610729610c77565b600073ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff1603610798576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161078f906115e8565b60405180910390fd5b6107a181611018565b50565b600033905090565b600073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff160361081b576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016108129061167a565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff160361088a576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016108819061170c565b60405180910390fd5b80600160008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055508173ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b9258360405161096891906112b8565b60405180910390a3505050565b6000610981848461069a565b90507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff81146109fb57818110156109ed576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016109e490611778565b60405180910390fd5b6109fa84848484036107ac565b5b50505050565b600073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff1603610a70576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610a679061180a565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1603610adf576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610ad69061189c565b60405180910390fd5b610aea8383836110de565b60008060008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905081811015610b70576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610b679061192e565b60405180910390fd5b8181036000808673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550816000808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600082825401925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef84604051610c5e91906112b8565b60405180910390a3610c718484846110e3565b50505050565b610c7f6107a4565b73ffffffffffffffffffffffffffffffffffffffff16610c9d610544565b73ffffffffffffffffffffffffffffffffffffffff1614610cf3576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610cea9061199a565b60405180910390fd5b565b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1603610d64576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610d5b90611a06565b60405180910390fd5b610d70600083836110de565b8060026000828254610d8291906114b0565b92505081905550806000808473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600082825401925050819055508173ffffffffffffffffffffffffffffffffffffffff16600073ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef83604051610e3391906112b8565b60405180910390a3610e47600083836110e3565b5050565b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1603610eba576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610eb190611a98565b60405180910390fd5b610ec6826000836110de565b60008060008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905081811015610f4c576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610f4390611b2a565b60405180910390fd5b8181036000808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190555081600260008282540392505081905550600073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef84604051610fff91906112b8565b60405180910390a3611013836000846110e3565b505050565b6000600560009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905081600560006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508173ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff167f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e060405160405180910390a35050565b505050565b505050565b600081519050919050565b600082825260208201905092915050565b60005b83811015611122578082015181840152602081019050611107565b60008484015250505050565b6000601f19601f8301169050919050565b600061114a826110e8565b61115481856110f3565b9350611164818560208601611104565b61116d8161112e565b840191505092915050565b60006020820190508181036000830152611192818461113f565b905092915050565b600080fd5b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b60006111ca8261119f565b9050919050565b6111da816111bf565b81146111e557600080fd5b50565b6000813590506111f7816111d1565b92915050565b6000819050919050565b611210816111fd565b811461121b57600080fd5b50565b60008135905061122d81611207565b92915050565b6000806040838503121561124a5761124961119a565b5b6000611258858286016111e8565b92505060206112698582860161121e565b9150509250929050565b60008115159050919050565b61128881611273565b82525050565b60006020820190506112a3600083018461127f565b92915050565b6112b2816111fd565b82525050565b60006020820190506112cd60008301846112a9565b92915050565b6000806000606084860312156112ec576112eb61119a565b5b60006112fa868287016111e8565b935050602061130b868287016111e8565b925050604061131c8682870161121e565b9150509250925092565b600060ff82169050919050565b61133c81611326565b82525050565b60006020820190506113576000830184611333565b92915050565b6000602082840312156113735761137261119a565b5b60006113818482850161121e565b91505092915050565b6000602082840312156113a05761139f61119a565b5b60006113ae848285016111e8565b91505092915050565b6113c0816111bf565b82525050565b60006020820190506113db60008301846113b7565b92915050565b600080604083850312156113f8576113f761119a565b5b6000611406858286016111e8565b9250506020611417858286016111e8565b9150509250929050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b6000600282049050600182168061146857607f821691505b60208210810361147b5761147a611421565b5b50919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b60006114bb826111fd565b91506114c6836111fd565b92508282019050808211156114de576114dd611481565b5b92915050565b7f45524332303a2064656372656173656420616c6c6f77616e63652062656c6f7760008201527f207a65726f000000000000000000000000000000000000000000000000000000602082015250565b60006115406025836110f3565b915061154b826114e4565b604082019050919050565b6000602082019050818103600083015261156f81611533565b9050919050565b7f4f776e61626c653a206e6577206f776e657220697320746865207a65726f206160008201527f6464726573730000000000000000000000000000000000000000000000000000602082015250565b60006115d26026836110f3565b91506115dd82611576565b604082019050919050565b60006020820190508181036000830152611601816115c5565b9050919050565b7f45524332303a20617070726f76652066726f6d20746865207a65726f2061646460008201527f7265737300000000000000000000000000000000000000000000000000000000602082015250565b60006116646024836110f3565b915061166f82611608565b604082019050919050565b6000602082019050818103600083015261169381611657565b9050919050565b7f45524332303a20617070726f766520746f20746865207a65726f20616464726560008201527f7373000000000000000000000000000000000000000000000000000000000000602082015250565b60006116f66022836110f3565b91506117018261169a565b604082019050919050565b60006020820190508181036000830152611725816116e9565b9050919050565b7f45524332303a20696e73756666696369656e7420616c6c6f77616e6365000000600082015250565b6000611762601d836110f3565b915061176d8261172c565b602082019050919050565b6000602082019050818103600083015261179181611755565b9050919050565b7f45524332303a207472616e736665722066726f6d20746865207a65726f20616460008201527f6472657373000000000000000000000000000000000000000000000000000000602082015250565b60006117f46025836110f3565b91506117ff82611798565b604082019050919050565b60006020820190508181036000830152611823816117e7565b9050919050565b7f45524332303a207472616e7366657220746f20746865207a65726f206164647260008201527f6573730000000000000000000000000000000000000000000000000000000000602082015250565b60006118866023836110f3565b91506118918261182a565b604082019050919050565b600060208201905081810360008301526118b581611879565b9050919050565b7f45524332303a207472616e7366657220616d6f756e742065786365656473206260008201527f616c616e63650000000000000000000000000000000000000000000000000000602082015250565b60006119186026836110f3565b9150611923826118bc565b604082019050919050565b600060208201905081810360008301526119478161190b565b9050919050565b7f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572600082015250565b60006119846020836110f3565b915061198f8261194e565b602082019050919050565b600060208201905081810360008301526119b381611977565b9050919050565b7f45524332303a206d696e7420746f20746865207a65726f206164647265737300600082015250565b60006119f0601f836110f3565b91506119fb826119ba565b602082019050919050565b60006020820190508181036000830152611a1f816119e3565b9050919050565b7f45524332303a206275726e2066726f6d20746865207a65726f2061646472657360008201527f7300000000000000000000000000000000000000000000000000000000000000602082015250565b6000611a826021836110f3565b9150611a8d82611a26565b604082019050919050565b60006020820190508181036000830152611ab181611a75565b9050919050565b7f45524332303a206275726e20616d6f756e7420657863656564732062616c616e60008201527f6365000000000000000000000000000000000000000000000000000000000000602082015250565b6000611b146022836110f3565b9150611b1f82611ab8565b604082019050919050565b60006020820190508181036000830152611b4381611b07565b905091905056fea26469706673582212204f5863a5c182fce47d84abd8a2c644c72bf23a527680e14ec57349d38f1cd1a564736f6c63430008110033";
                string code = await client.CodeAt(contractAddress, "latest");
                Assert.AreEqual(expectedDeployedCode, code);
                code = await client.CodeAt(contractAddress, "earliest");
                Assert.AreEqual("0x", code);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestCaseSource(nameof(urls))]
        public async Task TestEstimateGas(string url) {
            try {
                var client = new SequenceEthClient(url);
                BigInteger gas = await client.EstimateGas(new TransactionCall());
                Assert.Greater(gas, BigInteger.Zero);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestCaseSource(nameof(urls))]
        public async Task TestFeeHistory(string url) {
            try {
                var client = new SequenceEthClient(url);
                string blockCount = "0x11";
                string blockNumber = await client.BlockNumber();
                // Must have at least blockCount blocks of fee history
                while (blockNumber.HexStringToBigInteger() < blockCount.HexStringToBigInteger()) {
                    await Task.Delay((int)(blockTimeInSeconds * 1000));
                    blockNumber = await client.BlockNumber();
                }
                FeeHistoryResult feeHistory = await client.FeeHistory(blockCount, "latest", new int[] { });
                Assert.IsNotNull(feeHistory);
                Assert.AreEqual((int)blockCount.HexStringToBigInteger() + 1, feeHistory.baseFeePerGas.Count); // This includes the next block after the newest of the returned range
                Assert.AreEqual((int)blockCount.HexStringToBigInteger(), feeHistory.gasUsedRatio.Count);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        public async Task TestHeaderByHash(Block expectedBlock, string url)
        {
            try
            {
                var client = new SequenceEthClient(url);
                Block block = await client.HeaderByHash(expectedBlock.hash);
                Assert.AreEqual(expectedBlock.ToString(), block.ToString());
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestCaseSource(nameof(urls))]
        public async Task TestHeaderByNumber(string url)
        {
            try
            {
                Block startingBlock = new Block()
                {
                    number = "0x0",
                    nonce = "0x0000000000000000",
                    gasUsed = "0x0",
                };
                var client = new SequenceEthClient(url);

                Block block = await client.HeaderByNumber("earliest");

                Assert.AreEqual(startingBlock.number, block.number);
                Assert.AreEqual(startingBlock.nonce, block.nonce);
                Assert.AreEqual(startingBlock.gasUsed, block.gasUsed);

                await TestHeaderByHash(block, url);

                block = await client.HeaderByNumber("latest");

                await TestHeaderByHash(block, url);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        private static object[] networkIdCases =
        {
            new object[] { testnetUrl,  "31337"},
            new object[] { publicPolygonRpc, "137" },
        };

        [TestCaseSource(nameof(networkIdCases))]
        public async Task TestNetworkId(string url, string expected) {
            try {
                var client = new SequenceEthClient(url);
                string networkId = await client.NetworkId();
                Assert.AreEqual(expected, networkId);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        
        [TestCaseSource(nameof(urls))]
        public async Task TestNonceAt(string url) {
            try {
                var client = new SequenceEthClient(url);
                BigInteger nonce = await client.NonceAt(wallet1.GetAddress(), "earliest");
                Assert.AreEqual(BigInteger.Zero, nonce);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestSendRawTransaction() {
            try {
                var client = new SequenceEthClient(testnetUrl);

                BigInteger nonce = await wallet1.GetNonce(client);
                string chainId = await client.ChainID();
                EthTransaction transaction = new EthTransaction(nonce, 100, 30000000, wallet2.GetAddress(), 1, "", chainId);
                string tx = transaction.SignAndEncodeTransaction(wallet1);

                string result = await client.SendRawTransaction(tx);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.StartsWith("0x"));

                await client.WaitForTransactionReceipt(result); // Not waiting for the transaction to process will cause the next tests to fail as they would be submitting a duplicate transaction
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestCaseSource(nameof(urls))]
        public async Task TestSuggestGasPrice(string url) {
            try {
                var client = new SequenceEthClient(url);
                BigInteger gasPrice = await client.SuggestGasPrice();
                Assert.Greater(gasPrice, BigInteger.Zero);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestSuggestGasTipCap() {
            try {
                var client = new SequenceEthClient(publicPolygonRpc); // Testnet does not support this method
                BigInteger gasTipCap = await client.SuggestGasTipCap();
                Assert.Greater(gasTipCap, BigInteger.Zero);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestTransactionByHash() {
            try {
                var client = new SequenceEthClient(testnetUrl);

                BigInteger nonce = await wallet1.GetNonce(client);
                BigInteger gasPrice = 100;
                BigInteger gasLimit = 30000000;
                BigInteger value = 1;
                string data = "";
                string chainId = await client.ChainID();
                string encoded_signing = EthTransaction.RLPEncode(nonce, gasPrice, gasLimit, wallet2.GetAddress(), value, data, chainId);
                string signingHash = "0x" + SequenceCoder.KeccakHash(encoded_signing);
                (string v, string r, string s) = wallet1.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash), chainId);
                string tx = EthTransaction.RLPEncode(nonce, gasPrice, gasLimit, wallet2.GetAddress(), value, data, chainId, v, r, s);

                string result = await client.SendRawTransaction(tx);

                ClientTransaction clientTransaction = await client.TransactionByHash(result);

                Assert.IsNotNull(clientTransaction);
                Assert.AreEqual(result, clientTransaction.hash);
                Assert.AreEqual(nonce, clientTransaction.nonce.HexStringToBigInteger());
                Assert.AreEqual(gasPrice, clientTransaction.gasPrice.HexStringToBigInteger());
                Assert.AreEqual(gasLimit, clientTransaction.gas.HexStringToBigInteger());
                Assert.AreEqual(wallet2.GetAddress().Value, SequenceCoder.AddressChecksum(clientTransaction.to));
                Assert.AreEqual(value, clientTransaction.value.HexStringToBigInteger());
                Assert.AreEqual(data.EnsureHexPrefix(), clientTransaction.input);
                Assert.AreEqual(v, clientTransaction.v);
                Assert.AreEqual(r, clientTransaction.r);
                Assert.AreEqual(s, clientTransaction.s);
                Assert.AreEqual(wallet1.GetAddress().Value, SequenceCoder.AddressChecksum(clientTransaction.from));

                await client.WaitForTransactionReceipt(result); // Not waiting for the transaction to process will cause the next tests to fail as they would be submitting a duplicate transaction
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestTransactionCount() {
            try {
                var client = new SequenceEthClient(testnetUrl);

                Block earliestBlock = await client.BlockByNumber("earliest");

                BigInteger transactionCount = await client.TransactionCount(earliestBlock.hash);
                Assert.AreEqual(BigInteger.Zero, transactionCount);

                string transactionHash = await wallet1.SendTransaction(client, wallet2.GetAddress(), 1);
                await client.WaitForTransactionReceipt(transactionHash); // Wait for transaction to process so we have a block hash
                
                ClientTransaction clientTransaction = await client.TransactionByHash(transactionHash);
                Assert.IsNotNull(clientTransaction);
                Assert.IsNotNull(clientTransaction.blockHash);

                transactionCount = await client.TransactionCount(clientTransaction.blockHash);
                Assert.Greater(transactionCount, BigInteger.Zero);
            }
            catch (Exception ex) {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task TestWaitForTransactionReceipt()
        {
            try
            {
                EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
                EthWallet wallet2 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
                SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
                string result = await wallet.SendTransaction(client, wallet2.GetAddress(), 1);

                TransactionReceipt receipt = await client.WaitForTransactionReceipt(result);

                Assert.IsNotNull(receipt);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        // Note: for methods with optional parameters, we must still specify a value for each parameter
        // This is because the tests are created reflexively and would otherwise throw an exception
        private static readonly object[] errorCases = {
            new object[] { nameof(SequenceEthClient.BalanceAt), new object[] { validAddress, "latest" } },
            new object[] { nameof(SequenceEthClient.BlockByHash), new object[] { "some hash" } },
            new object[] { nameof(SequenceEthClient.BlockByNumber), new object[] { "latest" } },
            new object[] { nameof(SequenceEthClient.BlockNumber), null},
            new object[] { nameof(SequenceEthClient.BlockRange), new object[] { "latest", "latest" } },
            new object[] { nameof(SequenceEthClient.CallContract), new object[] { new object[] { "latest" } } },
            new object[] { nameof(SequenceEthClient.CallContract), new object[] { new object[] { "latest", BigInteger.One, "random stuff" } } },
            new object[] { nameof(SequenceEthClient.CallContract), new object[] { null } },
            new object[] { nameof(SequenceEthClient.ChainID), null},
            new object[] { nameof(SequenceEthClient.CodeAt), new object[] { validAddress, "latest" } },
            new object[] { nameof(SequenceEthClient.EstimateGas), new object[] { new TransactionCall() } },
            new object[] { nameof(SequenceEthClient.FeeHistory), new object[] { "latest", "latest", new int[] { } } },
            new object[] { nameof(SequenceEthClient.HeaderByHash), new object[] { "some hash" } },
            new object[] { nameof(SequenceEthClient.HeaderByNumber), new object[] { "latest" } },
            new object[] { nameof(SequenceEthClient.NetworkId), null},
            new object[] { nameof(SequenceEthClient.NonceAt), new object[] { validAddress, "latest" } },
            new object[] { nameof(SequenceEthClient.SendRawTransaction), new object[] { "transaction data" } },
            new object[] { nameof(SequenceEthClient.SuggestGasPrice), null},
            new object[] { nameof(SequenceEthClient.SuggestGasTipCap), null},
            new object[] { nameof(SequenceEthClient.TransactionByHash), new object[] { "some hash" } },
            new object[] { nameof(SequenceEthClient.TransactionCount), new object[] { "some hash" } },
            new object[] { nameof(SequenceEthClient.TransactionReceipt), new object[] { "some hash" } },
            new object[] { nameof(SequenceEthClient.WaitForTransactionReceipt), new object[] { "some hash", 1, 1 } },
        };

        [TestCaseSource(nameof(errorCases))]
        public async Task TestErrorResponse(string methodName, params object[] parameters) {
            try {
                var client = new SequenceEthClient(failingClient);
                var method = typeof(SequenceEthClient).GetMethod(methodName);
                var task = (Task)method.Invoke(client, parameters);
                await task.ConfigureAwait(false);
                Assert.Fail("Expected an exception but none was thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual(FailingRpcClient.ErrorMessage, ex.Message);
            }
        }
    }
}
