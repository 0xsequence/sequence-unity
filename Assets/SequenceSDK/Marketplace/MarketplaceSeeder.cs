using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.EmbeddedWallet.Tests;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class MarketplaceSeeder
    {
        private Chain _chain = Chain.Polygon;
        
        // [Test]
        [Timeout(1800000)]
        public async Task SeedMarketplace()
        {
            throw new Exception("Please only run this when you need to seed the marketplace for testing as it will create a lot of listings and offers, using a fair amount of gas and credits.");
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            bool done = false;
            testHarness.Login(async wallet =>
            {
                Address collection = new Address("0x079294e6ffec16234578c672fa3fbfd4b6c48640");
                ERC1155 universallyMintableNft = new ERC1155(collection);

                Address erc20UniversallyMintable = new Address("0x88e57238a23e2619fd42f479d546560b44c698fe");
                ERC20 universallyMintableToken = new ERC20(erc20UniversallyMintable);

                TransactionReturn mintResult = await wallet.SendTransaction(_chain, new Transaction[]
                {
                    new RawTransaction(collection, "0",
                        universallyMintableNft.MintBatch(wallet.GetWalletAddress(), new BigInteger[] {1, 2, 3, 4, 5}, new BigInteger[] {100000, 100000, 100000, 100000, 100000}).CallData),
                    new RawTransaction(erc20UniversallyMintable, "0",
                        universallyMintableToken.Mint(wallet.GetWalletAddress(), 1000000000000).CallData)
                });
                Assert.IsNotNull(mintResult);
                Assert.IsTrue(mintResult is SuccessfulTransactionReturn);

                List<Step> finalSteps = new List<Step>();
                Address USDC = new Address("0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359");
                Address[] possibleCurrencyAddresses = new Address[]
                    { USDC };
                Checkout checkout = new Checkout(wallet, _chain);
                for (int i = 0; i < 10; i++)
                {
                    Step[] steps = await checkout.GenerateListingTransaction(collection, (i % 5 + 1).ToString(), i + 1,
                        ContractType.ERC1155, possibleCurrencyAddresses.GetRandomObjectFromArray(), 1,
                        DateTime.Now + TimeSpan.FromDays(365));
                    if (i % 5 == 3)
                    {
                        await Task.Delay(1000);
                    }
                    foreach (var step in steps)
                    {
                        finalSteps.Add(step);
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    Step[] steps = await checkout.GenerateOfferTransaction(collection, (i % 5 + 1).ToString(), i + 1,
                        ContractType.ERC1155, possibleCurrencyAddresses.GetRandomObjectFromArray(), 1,
                        DateTime.Now + TimeSpan.FromDays(365));
                    if (i % 5 == 3)
                    {
                        await Task.Delay(1000);
                    }
                    foreach (var step in steps)
                    {
                        finalSteps.Add(step);
                    }
                }

                Transaction[] transactions = new Transaction[10];
                for (int j = 0; j < finalSteps.Count; j+=10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        transactions[i] = new RawTransaction(finalSteps[j+i].to, finalSteps[j+i].value, finalSteps[j+i].data);
                    }
                    TransactionReturn result = await wallet.SendTransaction(_chain, transactions);
                    Assert.IsNotNull(result);
                    // Assert.IsTrue(result is SuccessfulTransactionReturn);
                    if (result is SuccessfulTransactionReturn success)
                    {
                       Debug.Log(ChainDictionaries.BlockExplorerOf[_chain].AppendTrailingSlashIfNeeded() + "tx/" + success.txHash);
                       // Application.OpenURL(ChainDictionaries.BlockExplorerOf[_chain].AppendTrailingSlashIfNeeded() + "tx/" + success.txHash);
                    }
                }

                done = true;
            }, (error, method, email, methods) =>
            {
                Assert.Fail(error);
                done = true;
            });
            while (!done)
            {
                await Task.Yield();
            }
        }
    }
}