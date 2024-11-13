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
    public class MarketplaceCheckoutTests
    {
        private CollectibleOrder[] _collectibleOrders;

        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);
        
        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGetCheckoutOptions(int amount)
        {
            _collectibleOrders = await OrderFetcher.FetchListings();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < _collectibleOrders.Length; i++)
            {
                if (_collectibleOrders[i].order.status == OrderStatus.active)
                {
                    orders.Add(_collectibleOrders[i].order);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            CheckoutOptions options = await checkout.GetCheckoutOptions(orders.ToArray());
            
            Assert.IsNotNull(options);
            Assert.AreNotEqual(TransactionCrypto.unknown, options.crypto);
        }

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGenerateBuyTransaction(int amount)
        {
            _collectibleOrders = await OrderFetcher.FetchListings();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < _collectibleOrders.Length; i++)
            {
                if (_collectibleOrders[i].order.status == OrderStatus.active)
                {
                    orders.Add(_collectibleOrders[i].order);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateBuyTransaction(orders[i]);
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGenerateSellTransaction(int amount)
        {
            _collectibleOrders = await OrderFetcher.FetchOffers();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < _collectibleOrders.Length; i++)
            {
                if (_collectibleOrders[i].order.status == OrderStatus.active)
                {
                    orders.Add(_collectibleOrders[i].order);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }
            Checkout checkout = new Checkout(_testWallet, Chain.TestnetPolygonAmoy);

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateSellTransaction(orders[i]);
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task TestGenerateListingTransaction(int amount)
        {
            Checkout checkout = new Checkout(_testWallet, Chain.ArbitrumNova);
            ChainIndexer indexer = new ChainIndexer(Chain.ArbitrumNova);
            Address USDC = new Address("0x750ba8b76187092B0D1E87E28daaf484d1b5273b");
            
            GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                new GetTokenBalancesArgs(_testWallet.GetWalletAddress(), "0x0ee3af1874789245467e7482f042ced9c5171073"));
            Assert.IsNotNull(balancesReturn);
            TokenBalance[] balances = balancesReturn.balances;
            Assert.IsNotNull(balances);
            Assert.GreaterOrEqual(balances.Length, amount);

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateListingTransaction(new Address(balances[i].contractAddress), balances[i].tokenID.ToString(), 
                    balances[i].balance, balances[i].contractType.AsMarketplaceContractType(), USDC, 
                    1, DateTime.Now + TimeSpan.FromMinutes(30));
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGenerateOfferTransaction(int amount)
        {
            _collectibleOrders = await OrderFetcher.FetchListings();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < _collectibleOrders.Length; i++)
            {
                if (_collectibleOrders[i].order.status == OrderStatus.active)
                {
                    orders.Add(_collectibleOrders[i].order);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateOfferTransaction(new Address(orders[i].collectionContractAddress), orders[i].tokenId, 
                    BigInteger.Parse(orders[i].quantityAvailable), ContractType.ERC1155, new Address(orders[i].priceCurrencyAddress), 
                    1, DateTime.Now + TimeSpan.FromMinutes(30));
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [Test]
        public async Task TestCreateListingAndBuyIt()
        {
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            bool listingCreated = false;
            Address erc20UniversallyMintable = new Address("0x9d0d8dcba30c8b7241da84f922942c100eb1bddc");
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");
            Address initialWallet = null;
            bool bought = false;
            
            testHarness.Login(async wallet =>
            {
                try
                {
                    initialWallet = wallet.GetWalletAddress();
                    Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova);
                    ChainIndexer indexer = new ChainIndexer(Chain.ArbitrumNova);
                    
                    ERC1155 universallyMintable = new ERC1155(collection);
                    TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
                    {
                        new RawTransaction(collection, "0",
                            universallyMintable.Mint(wallet.GetWalletAddress(), 1, 10000).CallData)
                    });
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result is SuccessfulTransactionReturn);
                    await Task.Delay(3000); // Allow some time for the transaction to finalize and for the indexer to pick it up
            
                    GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                        new GetTokenBalancesArgs(wallet.GetWalletAddress(), collection));
                    Assert.IsNotNull(balancesReturn);
                    TokenBalance[] balances = balancesReturn.balances;
                    Assert.IsNotNull(balances);
                    Assert.Greater(balances.Length, 0);

                    
                    Step[] steps = await checkout.GenerateListingTransaction(new Address(balances[0].contractAddress), balances[0].tokenID.ToString(), 
                        balances[0].balance, balances[0].contractType.AsMarketplaceContractType(), erc20UniversallyMintable, 
                        1, DateTime.Now + TimeSpan.FromMinutes(30));
                    Assert.IsNotNull(steps);
                    Assert.Greater(steps.Length, 0);
                    
                    Transaction[] transactions = new Transaction[steps.Length];
                    for (int i = 0; i < steps.Length; i++)
                    {
                        transactions[i] = new RawTransaction(steps[i].to, steps[i].value, steps[i].data);
                    }

                    result = await wallet.SendTransaction(Chain.ArbitrumNova, transactions);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result is SuccessfulTransactionReturn);

                    await wallet.DropThisSession();

                    listingCreated = true;
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }, (error, method, email, methods) =>
            {
                Assert.Fail(error);
                return;
            });

            while (!listingCreated)
            {
                await Task.Yield();
            }
            
            testHarness.Login(async wallet =>
            {
                Assert.AreNotEqual(initialWallet, wallet.GetWalletAddress());
                
                ChainIndexer indexer = new ChainIndexer(Chain.ArbitrumNova);
                GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(wallet.GetWalletAddress(), collection));
                Assert.IsNotNull(balancesReturn);
                TokenBalance[] balances = balancesReturn.balances;
                BigInteger balance = BigInteger.Zero;
                if (balances.Length != 0)
                {
                    balance = balances[0].balance;
                }
                
                ERC20 universallyMintable = new ERC20(erc20UniversallyMintable);
                TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
                {
                    new RawTransaction(erc20UniversallyMintable, "0",
                        universallyMintable.Mint(wallet.GetWalletAddress(), 10000000).CallData)
                });
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SuccessfulTransactionReturn);

                Step[] steps = await FetchListingAndBuy(collection, wallet);
                Assert.Greater(steps.Length, 0);
                
                Transaction[] transactions = new Transaction[steps.Length];
                for (int i = 0; i < steps.Length; i++)
                {
                    transactions[i] = new RawTransaction(steps[i].to, steps[i].value, steps[i].data);
                }
                
                result = await wallet.SendTransaction(Chain.ArbitrumNova, transactions);
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SuccessfulTransactionReturn);
                
                balancesReturn = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(wallet.GetWalletAddress(), collection));
                Assert.IsNotNull(balancesReturn);
                TokenBalance[] newBalances = balancesReturn.balances;
                BigInteger newBalance = BigInteger.Zero;
                if (newBalances.Length != 0)
                {
                    newBalance = newBalances[0].balance;
                }
                Assert.Greater(newBalance, balance);
                
                bought = true;
            }, (error, method, email, methods) =>
            {
                Assert.Fail(error);
            });
            
            while (!bought)
            {
                await Task.Yield();
            }
        }

        private async Task<Step[]> FetchListingAndBuy(Address collection, SequenceWallet wallet, int retries = 0)
        {
            if (retries >= 5)
            {
                Assert.Fail("Failed to fetch and buy listing after 5 retries");
            }
            
            MarketplaceReader reader = new MarketplaceReader(Chain.ArbitrumNova);
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                _collectibleOrders = await reader.ListAllCollectibleListingsWithLowestPricedListingsFirst(collection);
                Assert.IsNotNull(_collectibleOrders);
                if (_collectibleOrders.Length > 0)
                {
                    Debug.Log($"No collectibles found. Retrying {retries + 1}...");
                    break;
                }
                await Task.Delay(5000);
            }
            Assert.Greater(_collectibleOrders.Length, 0);
                
            Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova);
            Step[] steps = null;
            try
            {
                steps = await checkout.GenerateBuyTransaction(_collectibleOrders[0].order);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("orders not valid orderIDs")) // Sometimes when the API has not fully been refreshed, we are given orders that have already been filled
                {
                    Debug.Log($"Encountered error: {e.Message}\nRetrying {retries + 1}...");
                    await Task.Delay(10000);
                    await FetchListingAndBuy(collection, wallet, retries + 1);
                }
                else
                {
                    Assert.Fail(e.Message);
                }
            }
            Assert.IsNotNull(steps);
            Assert.Greater(steps.Length, 0);
            return steps;
        }
        
        [Test]
        public async Task TestCreateOfferAndSellIt()
        {
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            bool offerCreated = false;
            Address erc20UniversallyMintable = new Address("0x9d0d8dcba30c8b7241da84f922942c100eb1bddc");
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");
            Address initialWallet = null;
            bool sold = false;
            
            testHarness.Login(async wallet =>
            {
                try
                {
                    initialWallet = wallet.GetWalletAddress();
                
                    ERC20 universallyMintable = new ERC20(erc20UniversallyMintable);
                    TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
                    {
                        new RawTransaction(erc20UniversallyMintable, "0",
                            universallyMintable.Mint(wallet.GetWalletAddress(), 10000000).CallData)
                    });
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result is SuccessfulTransactionReturn);
                    
                    Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova);

                    Step[] steps = await checkout.GenerateOfferTransaction(collection, "1", 
                        1, ContractType.ERC1155, erc20UniversallyMintable, 
                        1, DateTime.Now + TimeSpan.FromMinutes(30));
                    Assert.IsNotNull(steps);
                    Assert.Greater(steps.Length, 0);
                    
                    Transaction[] transactions = new Transaction[steps.Length];
                    for (int i = 0; i < steps.Length; i++)
                    {
                        transactions[i] = new RawTransaction(steps[i].to, steps[i].value, steps[i].data);
                    }

                    result = await wallet.SendTransaction(Chain.ArbitrumNova, transactions);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result is SuccessfulTransactionReturn);

                    await wallet.DropThisSession();

                    offerCreated = true;
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }, (error, method, email, methods) =>
            {
                Assert.Fail(error);
                return;
            });

            while (!offerCreated)
            {
                await Task.Yield();
            }
            
            testHarness.Login(async wallet =>
            {
                Assert.AreNotEqual(initialWallet, wallet.GetWalletAddress());
                    
                ChainIndexer indexer = new ChainIndexer(Chain.ArbitrumNova);
                
                GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(wallet.GetWalletAddress(), erc20UniversallyMintable));
                Assert.IsNotNull(balancesReturn);
                TokenBalance[] balances = balancesReturn.balances;
                BigInteger balance = BigInteger.Zero;
                if (balances.Length != 0)
                {
                    balance = balances[0].balance;
                }
                
                ERC1155 universallyMintable = new ERC1155(collection);
                TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
                {
                    new RawTransaction(collection, "0",
                        universallyMintable.Mint(wallet.GetWalletAddress(), 1, 10000).CallData)
                });
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SuccessfulTransactionReturn);
                await Task.Delay(3000); // Allow some time for the transaction to finalize and for the indexer to pick it up

                Step[] steps = await FetchOfferAndSell(collection, wallet);
                Assert.Greater(steps.Length, 0);
                
                Transaction[] transactions = new Transaction[steps.Length];
                for (int i = 0; i < steps.Length; i++)
                {
                    transactions[i] = new RawTransaction(steps[i].to, steps[i].value, steps[i].data);
                }
                
                result = await wallet.SendTransaction(Chain.ArbitrumNova, transactions);
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SuccessfulTransactionReturn);
                
                balancesReturn = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(wallet.GetWalletAddress(), erc20UniversallyMintable));
                Assert.IsNotNull(balancesReturn);
                TokenBalance[] newBalances = balancesReturn.balances;
                BigInteger newBalance = BigInteger.Zero;
                if (newBalances.Length != 0)
                {
                    newBalance = newBalances[0].balance;
                }
                Assert.Greater(newBalance, balance);
                
                sold = true;
            }, (error, method, email, methods) =>
            {
                Assert.Fail(error);
            });
            
            while (!sold)
            {
                await Task.Yield();
            }
        }
        
        

        private async Task<Step[]> FetchOfferAndSell(Address collection, SequenceWallet wallet, int retries = 0)
        {
            if (retries >= 5)
            {
                Assert.Fail("Failed to fetch and buy listing after 5 retries");
            }
            
            MarketplaceReader reader = new MarketplaceReader(Chain.ArbitrumNova);
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the offer might not have been picked up yet
            {
                _collectibleOrders = await reader.ListAllCollectibleOffersWithHighestPricedOfferFirst(collection);
                Assert.IsNotNull(_collectibleOrders);
                if (_collectibleOrders.Length > 0)
                {
                    Debug.Log($"No collectibles found. Retrying {retries + 1}...");
                    break;
                }
                await Task.Delay(5000);
            }
            Assert.Greater(_collectibleOrders.Length, 0);
                
            Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova);
            Step[] steps = null;
            try
            {
                steps = await checkout.GenerateSellTransaction(_collectibleOrders[0].order);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("orders not valid orderIDs")) // Sometimes when the API has not fully been refreshed, we are given orders that have already been filled
                {
                    Debug.Log($"Encountered error: {e.Message}\nRetrying {retries + 1}...");
                    await Task.Delay(10000);
                    await FetchOfferAndSell(collection, wallet, retries + 1);
                }
                else
                {
                    Assert.Fail(e.Message);
                }
            }
            Assert.IsNotNull(steps);
            Assert.Greater(steps.Length, 0);
            return steps;
        }

        public async Task SeedMarketplace()
        {
            throw new Exception("Please only run this when you need to seed the marketplace for testing as it will create a lot of listings and offers, using a fair amount of gas and credits.");
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            bool done = false;
            testHarness.Login(async wallet =>
            {
                Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");
                ERC1155 universallyMintableNft = new ERC1155(collection);

                Address erc20UniversallyMintable = new Address("0x9d0d8dcba30c8b7241da84f922942c100eb1bddc");
                ERC20 universallyMintableToken = new ERC20(erc20UniversallyMintable);

                TransactionReturn mintResult = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
                {
                    new RawTransaction(collection, "0",
                        universallyMintableNft.Mint(wallet.GetWalletAddress(), 1, 1000000000000).CallData),
                    new RawTransaction(erc20UniversallyMintable, "0",
                        universallyMintableToken.Mint(wallet.GetWalletAddress(), 1000000000000).CallData)
                });
                Assert.IsNotNull(mintResult);
                Assert.IsTrue(mintResult is SuccessfulTransactionReturn);

                List<Step> finalSteps = new List<Step>();
                Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova);
                for (int i = 0; i < 100; i++)
                {
                    Step[] steps = await checkout.GenerateListingTransaction(collection, "1", i + 1,
                        ContractType.ERC1155, erc20UniversallyMintable, 1,
                        DateTime.Now + TimeSpan.FromDays(365));
                    foreach (var step in steps)
                    {
                        finalSteps.Add(step);
                    }
                }

                for (int i = 0; i < 100; i++)
                {
                    Step[] steps = await checkout.GenerateOfferTransaction(collection, "1", i + 1,
                        ContractType.ERC1155, erc20UniversallyMintable, 1,
                        DateTime.Now + TimeSpan.FromDays(365));
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
                    TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, transactions);
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result is SuccessfulTransactionReturn);
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