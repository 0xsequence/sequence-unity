using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.EmbeddedWallet.Tests;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class MarketplaceCheckoutTests
    {
        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGetCheckoutOptions(int amount)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            CollectibleOrder[] collectibleOrders = await OrderFetcher.FetchListings();
            List<Order> orders = new List<Order>();
            for (int i = 0; i < collectibleOrders.Length; i++)
            {
                if (collectibleOrders[i].order.status == OrderStatus.active)
                {
                    orders.Add(collectibleOrders[i].order);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }

            Checkout checkout = new Checkout(_testWallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));

            CheckoutOptions options = await checkout.GetCheckoutOptions(orders.ToArray());

            Assert.IsNotNull(options);
            Assert.AreNotEqual(TransactionCrypto.unknown, options.crypto);
        }

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGenerateBuyTransaction(int amount)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Order[] ordersResponse = await OrderFetcher.FetchListingsForCollectible("1");
            List<Order> orders = new List<Order>();
            for (int i = 0; i < ordersResponse.Length; i++)
            {
                if (ordersResponse[i].status == OrderStatus.active)
                {
                    orders.Add(ordersResponse[i]);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }

            Assert.GreaterOrEqual(orders.Count, amount);

            Checkout checkout = new Checkout(_testWallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateBuyTransaction(orders[i], 1);
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGenerateSellTransaction(int amount)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Order[] orderResponse = await OrderFetcher.FetchOffersForCollectible("1");
            List<Order> orders = new List<Order>();
            for (int i = 0; i < orderResponse.Length; i++)
            {
                if (orderResponse[i].status == OrderStatus.active)
                {
                    orders.Add(orderResponse[i]);
                    if (orders.Count == amount)
                    {
                        break;
                    }
                }
            }

            Checkout checkout = new Checkout(_testWallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateSellTransaction(orders[i], 1);
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task TestGenerateListingTransaction(int amount)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Checkout checkout = new Checkout(_testWallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
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
                Step[] steps = await checkout.GenerateListingTransaction(new Address(balances[i].contractAddress),
                    balances[i].tokenID.ToString(),
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
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Checkout checkout = new Checkout(_testWallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            Address USDC = new Address("0x750ba8b76187092B0D1E87E28daaf484d1b5273b");
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateOfferTransaction(collection, "1",
                    1, ContractType.ERC1155, USDC,
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
                    await SeedWalletAndCreateListing(wallet, collection, erc20UniversallyMintable);

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
                await SubmitStepsAsTransaction(steps, wallet);
                await Task.Delay(
                    3000); // Allow some time for the transaction to finalize and for the indexer to pick it up

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
            }, (error, method, email, methods) => { Assert.Fail(error); });

            while (!bought)
            {
                await Task.Yield();
            }
        }

        private async Task SubmitStepsAsTransaction(Step[] steps, IWallet wallet)
        {
            Assert.IsNotNull(steps);
            Assert.Greater(steps.Length, 0);

            TransactionReturn result = await steps.SubmitAsTransactions(wallet, Chain.ArbitrumNova);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is SuccessfulTransactionReturn);
        }

        private async Task SeedWalletAndCreateListing(IWallet wallet, Address erc1155UniversallyMintable,
            Address currencyToken)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            ChainIndexer indexer = new ChainIndexer(Chain.ArbitrumNova);

            ERC1155 universallyMintable = new ERC1155(erc1155UniversallyMintable);
            TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
            {
                new RawTransaction(erc1155UniversallyMintable, "0",
                    universallyMintable.Mint(wallet.GetWalletAddress(), 1, 10000).CallData)
            });
            Assert.IsNotNull(result);
            Assert.IsTrue(result is SuccessfulTransactionReturn);
            await Task.Delay(3000); // Allow some time for the transaction to finalize and for the indexer to pick it up

            GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                new GetTokenBalancesArgs(wallet.GetWalletAddress(), erc1155UniversallyMintable));
            Assert.IsNotNull(balancesReturn);
            TokenBalance[] balances = balancesReturn.balances;
            Assert.IsNotNull(balances);
            Assert.Greater(balances.Length, 0);


            Step[] steps = await checkout.GenerateListingTransaction(new Address(balances[0].contractAddress),
                balances[0].tokenID.ToString(),
                balances[0].balance, balances[0].contractType.AsMarketplaceContractType(), currencyToken,
                1, DateTime.Now + TimeSpan.FromMinutes(30));
            await SubmitStepsAsTransaction(steps, wallet);
        }

        private async Task<Step[]> FetchListingAndBuy(Address collection, SequenceWallet wallet, int retries = 0)
        {
            if (retries >= 5)
            {
                Assert.Fail("Failed to fetch and buy listing after 5 retries");
            }

            CollectibleOrder[] collectibleOrders = await FetchListings(collection, Chain.ArbitrumNova);
            Assert.Greater(collectibleOrders.Length, 0);

            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            Step[] steps = null;
            try
            {
                steps = await checkout.GenerateBuyTransaction(collectibleOrders[0].order, 1);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(
                        "orders not valid orderIDs")) // Sometimes when the API has not fully been refreshed, we are given orders that have already been filled
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

        private async Task<CollectibleOrder[]> FetchListings(string collection, Chain chain, CollectiblesFilter filter = null)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            MarketplaceReader reader = new MarketplaceReader(chain, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            CollectibleOrder[] collectibleOrders = null;
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                collectibleOrders = await reader.ListAllCollectibleListingsWithLowestPricedListingsFirst(collection, filter);
                Assert.IsNotNull(collectibleOrders);
                if (collectibleOrders.Length > 0)
                {
                    Debug.Log($"No collectibles found. Retrying {i + 1}...");
                    break;
                }

                await Task.Delay(5000);
            }

            Assert.IsNotNull(collectibleOrders);
            Assert.Greater(collectibleOrders.Length, 0);
            
            return collectibleOrders;
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

                    WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
                    Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));

                    Step[] steps = await checkout.GenerateOfferTransaction(collection, "1",
                        1, ContractType.ERC1155, erc20UniversallyMintable,
                        1, DateTime.Now + TimeSpan.FromMinutes(30));
                    await SubmitStepsAsTransaction(steps, wallet);

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
                await Task.Delay(
                    3000); // Allow some time for the transaction to finalize and for the indexer to pick it up

                Step[] steps = await FetchOfferAndSell(collection, wallet);
                await SubmitStepsAsTransaction(steps, wallet);
                await Task.Delay(
                    3000); // Allow some time for the transaction to finalize and for the indexer to pick it up

                balancesReturn = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(initialWallet, erc20UniversallyMintable));
                Assert.IsNotNull(balancesReturn);
                TokenBalance[] newBalances = balancesReturn.balances;
                BigInteger newBalance = BigInteger.Zero;
                if (newBalances.Length != 0)
                {
                    newBalance = newBalances[0].balance;
                }

                Assert.Greater(newBalance, balance);

                sold = true;
            }, (error, method, email, methods) => { Assert.Fail(error); });

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

            CollectibleOrder[] collectibleOrders = await FetchOffers(collection, Chain.ArbitrumNova);

            Assert.Greater(collectibleOrders.Length, 0);

            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            Step[] steps = null;
            try
            {
                steps = await checkout.GenerateSellTransaction(collectibleOrders[0].order, 1);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(
                        "orders not valid orderIDs")) // Sometimes when the API has not fully been refreshed, we are given orders that have already been filled
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

        private async Task<CollectibleOrder[]> FetchOffers(string collection, Chain chain)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            MarketplaceReader reader = new MarketplaceReader(chain, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            CollectibleOrder[] collectibleOrders = null;
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                collectibleOrders = await reader.ListAllCollectibleOffersWithHighestPricedOfferFirst(collection);
                Assert.IsNotNull(collectibleOrders);
                if (collectibleOrders.Length > 0)
                {
                    Debug.Log($"No collectibles found. Retrying {i + 1}...");
                    break;
                }

                await Task.Delay(5000);
            }

            Assert.IsNotNull(collectibleOrders);
            Assert.Greater(collectibleOrders.Length, 0);
            
            return collectibleOrders;
        }

        [Test]
        public async Task TestCancelOrder()
        {
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            Address erc20UniversallyMintable = new Address("0x9d0d8dcba30c8b7241da84f922942c100eb1bddc");
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");
            bool cancelled = false;

            testHarness.Login(async wallet =>
            {
                await SeedWalletAndCreateListing(wallet, collection, erc20UniversallyMintable);

                Order myOrder = null;
                int retries = 5;
                for (int i = 0; i < retries; i++)
                {
                    CollectibleOrder[] collectibleOrders = await FetchListings(collection, Chain.ArbitrumNova, 
                        new CollectiblesFilter(true, "", null, null, new string[] {wallet.GetWalletAddress()}));
                    Assert.Greater(collectibleOrders.Length, 0);

                    myOrder = FindOrderCreatedByWallet(wallet.GetWalletAddress(), collectibleOrders);
                    if (myOrder != null)
                    {
                        break;
                    }
                    else
                    {
                        Debug.Log($"No order created by {wallet.GetWalletAddress()} found. Retrying {i + 1}...");
                    }
                }
                    
                if (myOrder == null)
                {
                    Assert.Fail($"Failed to find order created by wallet after {retries} retries");
                }

                WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
                Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
                Step[] steps = await checkout.GenerateCancelTransaction(collection, myOrder);
                await SubmitStepsAsTransaction(steps, wallet);
                    
                for (int i = 0; i < retries; i++)
                {
                    CollectibleOrder[] collectibleOrders = await FetchListings(collection, Chain.ArbitrumNova, 
                        new CollectiblesFilter(true, "", null, null, new string[] {wallet.GetWalletAddress()}));
                    Assert.Greater(collectibleOrders.Length, 0);

                    myOrder = FindOrderCreatedByWallet(wallet.GetWalletAddress(), collectibleOrders);
                    if (myOrder == null)
                    {
                        break;
                    }
                    else
                    {
                        Debug.Log($"My order {myOrder.orderId} is still found. Retrying {i + 1}...");
                    }
                }
                    
                if (myOrder != null)
                {
                    Assert.Fail($"Failed to see that order {myOrder.orderId} was cancelled after {retries} retries");
                }

                cancelled = true;
            }, (error, method, email, methods) => { Assert.Fail(error); });
            
            while (!cancelled)
            {
                await Task.Yield();
            }
        }

        private Order FindOrderCreatedByWallet(Address walletAddress, CollectibleOrder[] collectibleOrders)
        {
            foreach (var collectibleOrder in collectibleOrders)
            {
                Order order = collectibleOrder.order;
                if (order.createdBy == walletAddress)
                {
                    return order;
                }
            }

            return null;
        }

        [Test]
        public async Task TestCreateListingAndBuyWithNativeCurrency()
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            Address erc20UniversallyMintable = new Address("0x9d0d8dcba30c8b7241da84f922942c100eb1bddc");
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");
            bool listingCreated = false;
            bool bought = false;

            testHarness.Login(async wallet =>
            {
                await SeedWalletAndCreateListing(wallet, collection, new Address(Currency.NativeCurrencyAddress));

                await wallet.DropThisSession();
                
                listingCreated = true;
            }, (error, method, email, methods) => { Assert.Fail(error); });
            
            while (!listingCreated)
            {
                await Task.Yield();
            }
            
            string email = config.PlayFabEmail;
            var request = new LoginWithEmailAddressRequest { Email = email, Password = config.PlayFabPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request,
                result =>
                {
                    testHarness.LoginWithPlayFab(result, email, async wallet =>
                        {
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
                            
                            Order[] orders = await FetchListingsForCollectible(collection, "1", Chain.ArbitrumNova, 
                                new OrderFilter(null, null, new string[] {Currency.NativeCurrencyAddress}));
                            Assert.Greater(orders.Length, 0);

                            Order order = orders[0];
                            Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
                            Step[] steps = await checkout.GenerateBuyTransaction(order, 1);
                            await SubmitStepsAsTransaction(steps, wallet);
                            await Task.Delay(
                                3000); // Allow some time for the transaction to finalize and for the indexer to pick it up

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
                        },
                        (error, method, email, methods) => { Assert.Fail(error); });

                }, error => { Assert.Fail(error.ErrorMessage); });
            
            
            while (!bought)
            {
                await Task.Yield();
            }
        }
        
        private async Task<Order[]> FetchListingsForCollectible(string collection, string tokenId, Chain chain, OrderFilter filter = null)
        {
            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            MarketplaceReader reader = new MarketplaceReader(chain, HttpClient.UseHttpClientWithDevEnvironment(config.DevAPIKey));
            Order[] orders = null;
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                orders = await reader.ListAllListingsForCollectible(new Address(collection), tokenId, filter);
                Assert.IsNotNull(orders);
                if (orders.Length > 0)
                {
                    Debug.Log($"No orders found. Retrying {i + 1}...");
                    break;
                }

                await Task.Delay(5000);
            }

            Assert.IsNotNull(orders);
            Assert.Greater(orders.Length, 0);
            
            return orders;
        }
    }
}