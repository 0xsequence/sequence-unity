using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Authentication;
using Sequence.Contracts;
using Sequence.Utils;
using Sequence.WaaS.Tests;
using Sequence.Wallet;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WalletInteractionsTests
    {
        private Chain _chain = Chain.ArbitrumNova;
        
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
                    string signature = await wallet.SignMessage(_chain, message);
                    Assert.IsNotNull(signature);

                    IsValidMessageSignatureReturn isValid = await wallet.IsValidMessageSignature(_chain, message, signature);
                    Assert.IsNotNull(isValid);
                    Assert.IsTrue(isValid.isValid);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }

        [Test]
        public async Task TestDelayedEncode()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();

            testHarness.Login(async wallet =>
            {
                try
                {
                    string erc20Address = "0x079294e6ffec16234578c672fa3fbfd4b6c48640";
                    ChainIndexer indexer = new ChainIndexer(_chain);
                    GetTokenBalancesReturn balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(wallet.GetWalletAddress(), erc20Address));
                    BigInteger balance;
                    if (balanceReturn == null || balanceReturn.balances.Length == 0)
                    {
                        balance = 0;
                    }
                    else
                    {
                        balance = balanceReturn.balances[0].balance;
                    }

                    TransactionReturn transactionReturn = await wallet.SendTransaction(_chain,
                        new Transaction[]
                        {
                            new DelayedEncode(erc20Address, "0",
                                new DelayedEncodeData("mint(address,uint256)",
                                    new object[]
                                    {
                                        wallet.GetWalletAddress().Value, DecimalNormalizer.Normalize(1)
                                    }, "mint"))
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.IsTrue(transactionReturn is SuccessfulTransactionReturn);
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(wallet.GetWalletAddress(), erc20Address));
                    BigInteger balance2 = balanceReturn.balances[0].balance;
                    Assert.Greater(balance2, balance);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }

        [Test]
        public async Task TestTransferErc20()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            string toAddress = "0xc683a014955b75F5ECF991d4502427c8fa1Aa249";

            testHarness.Login(async wallet =>
            {
                try
                {
                    string erc20Address = "0x079294e6ffec16234578c672fa3fbfd4b6c48640";
                    ChainIndexer indexer = new ChainIndexer(_chain);
                    GetTokenBalancesReturn balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc20Address));
                    BigInteger balance;
                    if (balanceReturn == null || balanceReturn.balances.Length == 0)
                    {
                        balance = 0;
                    }
                    else
                    {
                        balance = balanceReturn.balances[0].balance;
                    }

                    TransactionReturn transactionReturn = await wallet.SendTransaction(_chain,
                        new Transaction[]
                        {
                            new DelayedEncode(erc20Address, "0",
                                new DelayedEncodeData("mint(address,uint256)",
                                    new object[]
                                    {
                                        wallet.GetWalletAddress().Value, DecimalNormalizer.Normalize(1)
                                    }, "mint")),
                            new SendERC20(erc20Address, toAddress, "1")
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.IsTrue(transactionReturn is SuccessfulTransactionReturn);
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc20Address));
                    BigInteger balance2 = balanceReturn.balances[0].balance;
                    Assert.Greater(balance2, balance);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }

        [Test]
        public async Task TestSendERC721()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            EOAWallet toWallet = new EOAWallet();
            string toAddress = toWallet.GetAddress().Value;

            testHarness.Login(async wallet =>
            {
                try
                {
                    string erc721Address = "0x88e57238a23e2619fd42f479d546560b44c698fe";
                    ChainIndexer indexer = new ChainIndexer(_chain);

                    TransactionReturn transactionReturn = await wallet.SendTransaction(_chain,
                        new Transaction[]
                        {
                            new DelayedEncode(erc721Address, "0",
                                new DelayedEncodeData("mint(address)",
                                    new object[]
                                    {
                                        wallet.GetWalletAddress().Value
                                    }, "mint")),
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.IsTrue(transactionReturn is SuccessfulTransactionReturn);
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    var balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(wallet.GetWalletAddress(), erc721Address));
                    BigInteger tokenId = balanceReturn.balances[0].tokenID;
                    
                    transactionReturn = await wallet.SendTransaction(_chain,
                        new Transaction[]
                        {
                            new SendERC721(erc721Address, toAddress, tokenId.ToString())
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.IsTrue(transactionReturn is SuccessfulTransactionReturn);
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    
                    balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc721Address));
                    Assert.AreEqual(tokenId, balanceReturn.balances[0].tokenID);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }
        
        [Test]
        public async Task TestSendERC1155()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            EOAWallet toWallet = new EOAWallet();
            string toAddress = toWallet.GetAddress().Value;

            testHarness.Login(async wallet =>
            {
                try
                {
                    string erc1155Address = "0x0ee3af1874789245467e7482f042ced9c5171073";
                    ChainIndexer indexer = new ChainIndexer(_chain);

                    TransactionReturn transactionReturn = await wallet.SendTransaction(_chain,
                        new Transaction[]
                        {
                            new DelayedEncode(erc1155Address, "0",
                                new DelayedEncodeData("mint(address,uint256,uint256,bytes)",
                                    new object[]
                                    {
                                        wallet.GetWalletAddress().Value, "1", "1", "0x0"
                                    }, "mint")),
                            new SendERC1155(erc1155Address, toAddress, new SendERC1155Values[]
                            {
                                new SendERC1155Values("1","1")
                            })
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.IsTrue(transactionReturn is SuccessfulTransactionReturn);
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    
                    var balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc1155Address));
                    Assert.AreEqual(BigInteger.One, balanceReturn.balances[0].tokenID);
                    Assert.AreEqual(BigInteger.One, balanceReturn.balances[0].balance);
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }

        [Test]
        public async Task TestRawTransactionsUsingContractWrappers()
        {
            var tcs = new TaskCompletionSource<bool>();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            EOAWallet toWallet = new EOAWallet();
            string toAddress = toWallet.GetAddress().Value;
            
            testHarness.Login(async wallet =>
            {
                try
                {
                    ERC20 erc20 = new ERC20("0x079294e6ffec16234578c672fa3fbfd4b6c48640");
                    ERC1155 erc1155 = new ERC1155("0x0ee3af1874789245467e7482f042ced9c5171073");
                    
                    ChainIndexer indexer = new ChainIndexer(_chain);

                    TransactionReturn transactionReturn = await wallet.SendTransaction(_chain,
                        new Transaction[]
                        {
                            new RawTransaction(erc20.GetAddress(), "0", erc20.Mint(toAddress, 1).CallData),
                            new RawTransaction(erc1155.Contract.GetAddress(), "0", erc1155.Mint(toAddress, 1, 1).CallData),
                        });
                    Assert.IsNotNull(transactionReturn);
                    Assert.IsTrue(transactionReturn is SuccessfulTransactionReturn);
                    await Task.Delay(5000); // Allow indexer some time to pick up transaction
                    
                    var balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress,  erc20.GetAddress()));
                    Assert.Greater(balanceReturn.balances[0].balance, BigInteger.Zero);
                    balanceReturn =
                        await indexer.GetTokenBalances(
                            new GetTokenBalancesArgs(toAddress, erc1155.Contract.GetAddress()));
                    Assert.Greater(balanceReturn.balances.Length, 0);
                    
                    tcs.TrySetResult(true);
                }
                catch (System.Exception e)
                {
                    tcs.TrySetException(e);
                }
            }, (error, method, email) =>
            {
                tcs.TrySetException(new Exception(error));
            });

            await tcs.Task;
        }
    }
}
