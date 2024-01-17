using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Sequence.Wallet;
using Sequence.ABI;
using Sequence.Provider;
using System.Text;
using NBitcoin.Secp256k1;
using System;
using System.Linq;
using Sequence;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using Sequence.Transactions;
using Sequence.Contracts;
using Sequence.Utils;
using Sequence.WaaS;
using ContractDeploymentResult = Sequence.Contracts.ContractDeploymentResult;
using IWallet = Sequence.Wallet.IWallet;

public class EthWalletTests
{
    const string privKey0 = "0xabc0000000000000000000000000000000000000000000000000000000000001";
    const string privKey1 = "0xabc0000000000000000000000000000000000000000000000000000000000002";
    const string privKey2 = "0xabc0000000000000000000000000000000000000000000000000000000000003";
    const string privKey3 = "0xabc0000000000000000000000000000000000000000000000000000000000004";
    const string privKey4 = "0xabc0000000000000000000000000000000000000000000000000000000000005";
    const string privKey5 = "0xabc0000000000000000000000000000000000000000000000000000000000006";

    //ERC20 Mock
    string bytecode_ERC20Mock =
        "0x608060405234801561001057600080fd5b50610997806100206000396000f3fe608060405234801561001057600080fd5b50600436106100be5760003560e01c80633950935111610076578063a457c2d71161005b578063a457c2d7146102f0578063a9059cbb14610329578063dd62ed3e14610362576100be565b8063395093511461028457806370a08231146102bd576100be565b806323b872dd116100a757806323b872dd1461012a5780632e72102f1461016d578063378934b41461024b576100be565b8063095ea7b3146100c357806318160ddd14610110575b600080fd5b6100fc600480360360408110156100d957600080fd5b5073ffffffffffffffffffffffffffffffffffffffff813516906020013561039d565b604080519115158252519081900360200190f35b6101186103b3565b60408051918252519081900360200190f35b6100fc6004803603606081101561014057600080fd5b5073ffffffffffffffffffffffffffffffffffffffff8135811691602081013590911690604001356103b9565b6102496004803603606081101561018357600080fd5b81019060208101813564010000000081111561019e57600080fd5b8201836020820111156101b057600080fd5b803590602001918460208302840111640100000000831117156101d257600080fd5b9193909273ffffffffffffffffffffffffffffffffffffffff8335169260408101906020013564010000000081111561020a57600080fd5b82018360208201111561021c57600080fd5b8035906020019184602083028401116401000000008311171561023e57600080fd5b509092509050610417565b005b6102496004803603604081101561026157600080fd5b5073ffffffffffffffffffffffffffffffffffffffff8135169060200135610509565b6100fc6004803603604081101561029a57600080fd5b5073ffffffffffffffffffffffffffffffffffffffff8135169060200135610517565b610118600480360360208110156102d357600080fd5b503573ffffffffffffffffffffffffffffffffffffffff1661055a565b6100fc6004803603604081101561030657600080fd5b5073ffffffffffffffffffffffffffffffffffffffff8135169060200135610582565b6100fc6004803603604081101561033f57600080fd5b5073ffffffffffffffffffffffffffffffffffffffff81351690602001356105c5565b6101186004803603604081101561037857600080fd5b5073ffffffffffffffffffffffffffffffffffffffff813581169160200135166105d2565b60006103aa33848461060a565b50600192915050565b60025490565b60006103c68484846106b9565b73ffffffffffffffffffffffffffffffffffffffff841660009081526001602090815260408083203380855292529091205461040d91869161040890866107ac565b61060a565b5060019392505050565b60005b818110156105015785858281811061042e57fe5b9050602002013573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1663a9059cbb8585858581811061047357fe5b905060200201356040518363ffffffff1660e01b8152600401808373ffffffffffffffffffffffffffffffffffffffff16815260200182815260200192505050602060405180830381600087803b1580156104cd57600080fd5b505af11580156104e1573d6000803e3d6000fd5b505050506040513d60208110156104f757600080fd5b505060010161041a565b505050505050565b6105138282610823565b5050565b33600081815260016020908152604080832073ffffffffffffffffffffffffffffffffffffffff8716845290915281205490916103aa91859061040890866108e6565b73ffffffffffffffffffffffffffffffffffffffff1660009081526020819052604090205490565b33600081815260016020908152604080832073ffffffffffffffffffffffffffffffffffffffff8716845290915281205490916103aa91859061040890866107ac565b60006103aa3384846106b9565b73ffffffffffffffffffffffffffffffffffffffff918216600090815260016020908152604080832093909416825291909152205490565b73ffffffffffffffffffffffffffffffffffffffff821661062a57600080fd5b73ffffffffffffffffffffffffffffffffffffffff831661064a57600080fd5b73ffffffffffffffffffffffffffffffffffffffff808416600081815260016020908152604080832094871680845294825291829020859055815185815291517f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b9259281900390910190a3505050565b73ffffffffffffffffffffffffffffffffffffffff82166106d957600080fd5b73ffffffffffffffffffffffffffffffffffffffff831660009081526020819052604090205461070990826107ac565b73ffffffffffffffffffffffffffffffffffffffff808516600090815260208190526040808220939093559084168152205461074590826108e6565b73ffffffffffffffffffffffffffffffffffffffff8084166000818152602081815260409182902094909455805185815290519193928716927fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef92918290030190a3505050565b60008282111561081d57604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601760248201527f536166654d617468237375623a20554e444552464c4f57000000000000000000604482015290519081900360640190fd5b50900390565b73ffffffffffffffffffffffffffffffffffffffff821661084357600080fd5b60025461085090826108e6565b60025573ffffffffffffffffffffffffffffffffffffffff821660009081526020819052604090205461088390826108e6565b73ffffffffffffffffffffffffffffffffffffffff83166000818152602081815260408083209490945583518581529351929391927fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef9281900390910190a35050565b60008282018381101561095a57604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601660248201527f536166654d617468236164643a204f564552464c4f5700000000000000000000604482015290519081900360640190fd5b939250505056fea26469706673582212201e8282683b3b9580e5722aa29d3e976acdd4cd35c3e88cdd1abb688867d0547a64736f6c63430007040033";

    string zeroAddress = "0x0";
    BigInteger gasPrice_ERC20Mock = 100;
    BigInteger gasLimit_ERC20Mock = 30000000;

    [Test]
    public void TestChain_AddressesTests()
    {
        string address_0_expected = "0xc683a014955b75F5ECF991d4502427c8fa1Aa249";
        EthWallet wallet0 = new EthWallet(privKey0);
        string address_0 = wallet0.GetAddress();
        Debug.Log("address 0 from wallet: " + address_0);
        CollectionAssert.AreEqual(address_0_expected, address_0);


        string address_1_expected = "0x1099542D7dFaF6757527146C0aB9E70A967f71C0";
        EthWallet wallet1 = new EthWallet(privKey1);
        string address_1 = wallet1.GetAddress();
        Debug.Log("address 1 from wallet: " + address_1);
        CollectionAssert.AreEqual(address_1_expected, address_1);

        string address_2_expected = "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa";
        EthWallet wallet2 = new EthWallet(privKey2);
        string address_2 = wallet2.GetAddress();
        Debug.Log("address 2 from wallet: " + address_2);
        CollectionAssert.AreEqual(address_2_expected, address_2);


        string address_3_expected = "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153";
        EthWallet wallet3 = new EthWallet(privKey3);
        string address_3 = wallet3.GetAddress();
        Debug.Log("address 3 from wallet: " + address_3);
        CollectionAssert.AreEqual(address_3_expected, address_3);


        string address_4_expected = "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94";
        EthWallet wallet4 = new EthWallet(privKey4);
        string address_4 = wallet4.GetAddress();
        Debug.Log("address 4 from wallet: " + address_4);
        CollectionAssert.AreEqual(address_4_expected, address_4);

        string address_5_expected = "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C";
        EthWallet wallet5 = new EthWallet(privKey5);
        string address_5 = wallet5.GetAddress();
        Debug.Log("address 5 from wallet: " + address_5);
        CollectionAssert.AreEqual(address_5_expected, address_5);


    }


    [Test]
    public async Task TestChain_TransactionTests()
    {
        //{ from: account0Address, to: account1Address, value: "12300000000000000000", gasLimit: 100000, gasPrice: 100 } 
        try
        {
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
            string to = "0x1099542D7dFaF6757527146C0aB9E70A967f71C0";
            BigInteger value = 12300000000;
            EthTransaction transaction =
                await new GasLimitEstimator(client, wallet.GetAddress()).BuildTransaction(to, null, value);
            string result = await wallet.SendTransaction(client, transaction);
            Assert.IsNotEmpty(result);

            await client
                .WaitForTransactionReceipt(
                    result); // Not waiting for the transaction to process will cause the next tests to fail as they would be submitting a duplicate transaction
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestChain_BalanceReducesAfterTransaction()
    {
        try
        {
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
            string to = "0x1099542D7dFaF6757527146C0aB9E70A967f71C0";
            BigInteger value = 12300000000;

            BigInteger balancePreTransaction = await wallet.GetBalance(client);

            TransactionReceipt receipt = await wallet.SendTransactionAndWaitForReceipt(client, to, null, value);

            BigInteger balancePostTransaction = await wallet.GetBalance(client);

            Assert.Less(balancePostTransaction, balancePreTransaction);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestChain_DeployERC20Mock_Tests()
    {

        try
        {
            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            ContractDeploymentResult result = await ContractDeployer.Deploy(client, wallet, bytecode_ERC20Mock,
                gasPrice_ERC20Mock, gasLimit_ERC20Mock);
            TransactionReceipt receipt = result.Receipt;

            Assert.IsNotNull(receipt.contractAddress);
            Assert.AreEqual(receipt.contractAddress, result.DeployedContractAddress.Value);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }

    }

    [Test]
    public async Task TestChain_ERC20Mock_MockMint_Test()
    {
        try
        {
            //Deploy First
            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            ContractDeploymentResult deployResult = await ContractDeployer.Deploy(client, wallet, bytecode_ERC20Mock,
                gasPrice_ERC20Mock, gasLimit_ERC20Mock);
            TransactionReceipt receipt = deployResult.Receipt;

            Assert.IsNotNull(receipt.contractAddress);
            Assert.AreEqual(receipt.contractAddress, deployResult.DeployedContractAddress.Value);

            //Interaction (mock mint)


            /*
             * function mockMint(address _address, uint256 _amount) public {
                    _mint(_address, _amount);
                }
             */

            EthWallet wallet2 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
            Contract mockERC20 = new Contract(receipt.contractAddress);
            string result = await mockERC20.SendTransactionMethod(wallet2, client, 0,
                "mockMint(address , uint256)",
                wallet2.GetAddress(), 1);
            Assert.IsNotNull(result);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestChain_ERC20Mock_Interactions_Test()
    {

    }

    [Test]
    public void EIP155SigningTxTest()
    {
        //Example from https://eips.ethereum.org/EIPS/eip-155#parameters
        //Consider a transaction with nonce = 9, gasprice = 20 * 10**9, startgas = 21000, to = 0x3535353535353535353535353535353535353535, value = 10**18, data='' (empty).
        //1. Signing data
        string expected_signing_data =
            "0xec098504a817c800825208943535353535353535353535353535353535353535880de0b6b3a764000080018080";
        string encoded_signing_data = EthTransaction.RLPEncode(9, 20000000000, 21000,
            "0x3535353535353535353535353535353535353535", 1000000000000000000, "", "1");
        CollectionAssert.AreEqual(expected_signing_data, encoded_signing_data);

        //signing hash
        string expected_signing_hash = "0xdaf5a779ae972f972197303d7b574746c7ef83eadac0f2791ad23db92e4c8e53";
        string sigining_hash = "0x" + SequenceCoder.KeccakHash(expected_signing_data);
        CollectionAssert.AreEqual(expected_signing_hash, sigining_hash);

        //the use of 37 instead of 27. The signed tx would become:

        string expected_signed_transaction =
            "0xf86c098504a817c800825208943535353535353535353535353535353535353535880de0b6b3a76400008025a028ef61340bd939bc2195fe537567866003e1a15d3c71ff63e1590620aa636276a067cbe9d8997f761aecb703304b3800ccf555c9f3dc64214b297fb1966a3b6d83";
        EthWallet wallet = new EthWallet("0x4646464646464646464646464646464646464646464646464646464646464646");
        (string v, string r, string s) =
            wallet.SignTransaction(SequenceCoder.HexStringToByteArray(expected_signing_hash), "1");
        int id = "1".HexStringToInt();
        int vInt = v.HexStringToInt();
        Debug.Log(id);
        Debug.Log(vInt);
        string encoded_signed_transaction = EthTransaction.RLPEncode(9, 20000000000, 21000,
            "0x3535353535353535353535353535353535353535", 1000000000000000000, "", "1", v, r, s);
        CollectionAssert.AreEqual(expected_signed_transaction, encoded_signed_transaction);
    }

    [Test]
    public void TestWalletRandom()
    {
        EthWallet wallet = new EthWallet();
        Assert.NotNull(wallet);

    }

    [Test]
    public async Task TestWalletSignMessage()
    {
        EthWallet wallet = new EthWallet();

        string address = wallet.GetAddress();
        Assert.NotNull(address);

        string sig = await wallet.SignMessage("hi");
        Assert.NotNull(sig);

        bool valid = await wallet.IsValidSignature(sig, "hi");
        Assert.IsTrue(valid);
    }

    private static IEnumerable<object[]> iWalletTestCases()
    {
        // TOdo fix test
        // var adapter = WaaSToWalletAdapter.CreateAsync(new WaaSWallet(
        //     "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA")).Result;
        yield return new object[] { new EthWallet(), "SDK by Horizon" };
        // yield return new object[] { adapter, "SDK by Horizon" };
        yield return new object[] { new EthWallet(), "" };
        // yield return new object[] { adapter, "" };
        yield return new object[] { new EthWallet(), DecodeABITests.longMultiLineString };
        // yield return new object[] { adapter, DecodeABITests.longMultiLineString };
    }


    [TestCaseSource(nameof(iWalletTestCases))]
    public async Task TestWalletSignMessageWithChainId(IWallet wallet, string message)
    {
        string address = wallet.GetAddress();
        Assert.NotNull(address);

        string sig = await wallet.SignMessage(message, Chain.Polygon.AsHexString());
        Assert.NotNull(sig);

        bool valid = await wallet.IsValidSignature(sig, message, chain: Chain.Polygon);
        Assert.IsTrue(valid);
    }

    [Test]
    public async Task TestWalletSignMessageExistingPrefix()
    {
        EthWallet wallet = new EthWallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
        CollectionAssert.AreEqual(SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"), wallet.privKey.sec.ToBytes());

        string address = wallet.GetAddress();
        CollectionAssert.AreEqual("0x2AD3Df4A43445545e486a5c62F98Cee22d500bdf", address);

        byte[] _19 = SequenceCoder.HexStringToByteArray("19");
        byte[] testMessage = Encoding.ASCII.GetBytes("Ethereum Signed Message:\n" +"this is a test".Length + "this is a test");
        testMessage = _19.Concat(testMessage).ToArray();
        string sig = await wallet.SignMessage(testMessage);

        Assert.AreEqual("0x45c666ac1fc5faae5639014d2c163c1ac4863fb78a4bd23c3785f7db99cf553666191da4cad5968d018287e784ceabc7f5565b5375a4b7e35cba897d0b666f0f1b", sig);
    }

    [Test]
    public async Task TestWalletSignMessageFromPrivateKey()
    {

        EthWallet wallet = new EthWallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
         CollectionAssert.AreEqual(  SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"), wallet.privKey.sec.ToBytes());

        string address = wallet.GetAddress();
        CollectionAssert.AreEqual("0x2AD3Df4A43445545e486a5c62F98Cee22d500bdf", address);


        byte[] testMessage = Encoding.ASCII.GetBytes("this is a test");

        string sig = await wallet.SignMessage(testMessage);

        Assert.AreEqual("0x45c666ac1fc5faae5639014d2c163c1ac4863fb78a4bd23c3785f7db99cf553666191da4cad5968d018287e784ceabc7f5565b5375a4b7e35cba897d0b666f0f1b", sig);
       
    }

    [Test]
    public async Task TestWalletSignAndRecover()
    {
        EthWallet wallet = new EthWallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
        CollectionAssert.AreEqual(SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"), wallet.privKey.sec.ToBytes());

        string address = wallet.GetAddress();
        CollectionAssert.AreEqual("0x2AD3Df4A43445545e486a5c62F98Cee22d500bdf", address);


        byte[] testMessage = Encoding.ASCII.GetBytes("this is a test");

        string sig = await wallet.SignMessage(testMessage);

        string recoveredAddr = wallet.Recover("this is a test", sig);

        Assert.AreEqual(address, recoveredAddr);
    }

    [Test]
    public async Task TestSendTransactionBatchAndWaitForReceipts()
    {
        EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
        string recipient1 = "0x1099542D7dFaF6757527146C0aB9E70A967f71C0";
        string recipient2 = "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa";
        BigInteger startingBalance = await client.BalanceAt(wallet.GetAddress());
        BigInteger startingBalance1 = await client.BalanceAt(recipient1);
        BigInteger startingBalance2 = await client.BalanceAt(recipient2);

        EthTransaction transaction1 = await TransferEth.CreateTransaction(client, wallet, recipient1, 1000);
        EthTransaction transaction2 = await TransferEth.CreateTransaction(client, wallet, recipient2, 1000);
        EthTransaction[] transactions = new EthTransaction[] { transaction1, transaction2 };

        TransactionReceipt[] receipts = await wallet.SendTransactionBatchAndWaitForReceipts(client, transactions);
        
        BigInteger endingBalance = await client.BalanceAt(wallet.GetAddress());
        BigInteger endingBalance1 = await client.BalanceAt(recipient1);
        BigInteger endingBalance2 = await client.BalanceAt(recipient2);
        
        Assert.Less(endingBalance, startingBalance);
        Assert.Greater(endingBalance1, startingBalance1);
        Assert.Greater(endingBalance2, startingBalance2);
    }

    [Test]
    public async Task TestSendTransactionBatchAndWaitForReceipts_emptyBatch()
    {
        EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
        EthTransaction[] transactions = new EthTransaction[] {};

        TransactionReceipt[] receipts = await wallet.SendTransactionBatchAndWaitForReceipts(client, transactions);
        
        Assert.AreEqual(0, receipts.Length);
    }

}
