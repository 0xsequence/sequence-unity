using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Authentication;
using Sequence.Utils;
using Sequence.WaaS.Tests;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WalletInteractionsTests
    {
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
                    string signature = await wallet.SignMessage(Chain.ArbitrumNova, message);
                    Assert.IsNotNull(signature);

                    IsValidMessageSignatureReturn isValid = await wallet.IsValidMessageSignature(Chain.ArbitrumNova, message, signature);
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
                    ChainIndexer indexer = new ChainIndexer(Chain.ArbitrumNova);
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

                    TransactionReturn transactionReturn = await wallet.SendTransaction(Chain.ArbitrumNova,
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
    }
}
