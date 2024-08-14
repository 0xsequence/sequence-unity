using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Ethereum.Tests;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WalletAdapterToEOAInteractionTests
    {
        private Chain _chain = Chain.ArbitrumNova;
        
        [Test]
        public async Task TestGetAddress()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();

            testHarness.Login(async wallet =>
            {
                try
                {
                    SequenceWalletToEOAWalletAdapter adapter = new SequenceWalletToEOAWalletAdapter(wallet);
                    Assert.AreEqual(wallet.GetWalletAddress(), adapter.GetAddress());
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }
        
        [TestCase("Hi")]
        [TestCase(@"This is a much
longer message that spans
multiple lines. and has funky characters like this one $ and this one ~ and all of these '~!@#$%^&*()_+{}|:""<>,.?/")]
        public async Task TestSignMessage(string message)
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            
            await testHarness.Login(async wallet =>
            {
                try
                {
                    SequenceWalletToEOAWalletAdapter adapter = new SequenceWalletToEOAWalletAdapter(wallet);
                    string signature = await adapter.SignMessage(message, _chain.AsHexString());
                    Assert.IsNotNull(signature);

                    bool isValid = await adapter.IsValidSignature(signature, message, _chain);
                    Assert.IsTrue(isValid);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }

        [Test]
        public async Task TestMintErc20()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();

            testHarness.Login(async wallet =>
            {
                try
                {
                    SequenceWalletToEOAWalletAdapter adapter = new SequenceWalletToEOAWalletAdapter(wallet);
                    IEthClient client = new SequenceEthClient(_chain);
                    string erc20Address = "0x079294e6ffec16234578c672fa3fbfd4b6c48640";
                    ERC20 erc20 = new ERC20(erc20Address);
                    ChainIndexer indexer = new ChainIndexer(_chain);
                    GetTokenBalancesReturn balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(adapter.GetAddress(), erc20Address));
                    BigInteger balance;
                    if (balanceReturn == null || balanceReturn.balances.Length == 0)
                    {
                        balance = 0;
                    }
                    else
                    {
                        balance = balanceReturn.balances[0].balance;
                    }

                    TransactionReceipt transactionReturn = await erc20.Mint(adapter.GetAddress(), 1)
                        .SendTransactionMethodAndWaitForReceipt(adapter, client);
                    Assert.IsNotNull(transactionReturn);
                    
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(adapter.GetAddress(), erc20Address));
                    BigInteger balance2 = balanceReturn.balances[0].balance;
                    Assert.Greater(balance2, balance);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }

        [Test]
        public async Task TestSendBatchTransactions()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            EOAWallet toWallet = new EOAWallet();
            string toAddress = toWallet.GetAddress();

            testHarness.Login(async wallet =>
            {
                try
                {
                    SequenceWalletToEOAWalletAdapter adapter = new SequenceWalletToEOAWalletAdapter(wallet);
                    IEthClient client = new SequenceEthClient(_chain);
                    string erc20Address = "0x079294e6ffec16234578c672fa3fbfd4b6c48640";
                    ERC20 erc20 = new ERC20(erc20Address);
                    string erc1155Address = "0x0ee3af1874789245467e7482f042ced9c5171073";
                    ERC1155 erc1155 = new ERC1155(erc1155Address);
                    ChainIndexer indexer = new ChainIndexer(_chain);

                    EthTransaction mintErc20 = await erc20.Mint(toAddress, 1)
                        .Create(client, new ContractCall(adapter.GetAddress()));
                    EthTransaction erc1155Mint = await erc1155.Mint(toAddress, 1, 1)
                        .Create(client, new ContractCall(adapter.GetAddress()));

                    TransactionReceipt[] transactionReturn = await adapter.SendTransactionBatchAndWaitForReceipts(client,
                        new EthTransaction[]
                        {
                            mintErc20, erc1155Mint
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.AreEqual(1, transactionReturn.Length);
                    
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    var balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc20Address));
                    BigInteger balance = balanceReturn.balances[0].balance;
                    Assert.AreEqual(BigInteger.One, balance);
                    balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc1155Address));
                    BigInteger balance2 = balanceReturn.balances[0].balance;
                    Assert.AreEqual(BigInteger.One, balance2);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email, methods) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }
    }
}