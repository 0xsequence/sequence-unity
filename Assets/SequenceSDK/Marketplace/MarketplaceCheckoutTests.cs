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
using Random = UnityEngine.Random;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.Marketplace
{
    public class MarketplaceCheckoutTests
    {
        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);
        private Chain _chain = Chain.Polygon;

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGetCheckoutOptions_Marketplace(int amount)
        {
            CollectibleOrder[] collectibleOrders = await OrderFetcher.FetchListings(_chain, "0x0ee3af1874789245467e7482f042ced9c5171073");
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

            Checkout checkout = new Checkout(_testWallet, _chain);

            CheckoutOptions options = await checkout.GetCheckoutOptions(orders.ToArray());

            Assert.IsNotNull(options);
            Assert.AreNotEqual(TransactionCrypto.unknown, options.crypto);
        }

        [TestCase(new [] { "1" }, new long[] { 1 }, null)]
        [TestCase(new [] { "1", "2", "3" }, new long[] { 1, 2, 3 }, null)]
        [TestCase(new string[]{}, new long[]{}, "Must provide at least one tokenId and amount")]
        [TestCase(new string[] {"1"}, new long[] {-1}, "Amount must be larger than 0")]
        [TestCase(new string[] {"1", "2"}, new long[] {1, 0}, "Amount must be larger than 0")]
        [TestCase(new string[] {""}, new long[] {1}, "TokenId is invalid")]
        [TestCase(new string[] {"something random that isn't a token id"}, new long[] {1}, "TokenId is invalid")]
        public async Task TestGetCheckoutOptions_PrimarySale_ERC1155(string[] tokenIds, long[] amounts, string expectedException)
        {
            Dictionary<string, BigInteger> amountsByTokenId = new Dictionary<string, BigInteger>();
            int length = tokenIds.Length;
            Assert.AreEqual(length, amounts.Length, $"Invalid test setup. {nameof(tokenIds)} and {nameof(amounts)} must have the same length");
            for (int i = 0; i < length; i++)
            {
                amountsByTokenId[tokenIds[i]] = amounts[i];
            }

            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);
            ERC1155Sale sale = new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b");
            ERC1155 collection = new ERC1155("0xdeb398f41ccd290ee5114df7e498cf04fac916cb");

            if (!string.IsNullOrWhiteSpace(expectedException))
            {
                try
                {
                    CheckoutOptions options = await checkout.GetCheckoutOptions(sale, collection, amountsByTokenId);
                    Assert.Fail("Expected exception but none was encountered");
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.Message.Contains(expectedException));
                }
            }
            else
            {
                CheckoutOptions options = await checkout.GetCheckoutOptions(sale, collection, amountsByTokenId);
                Assert.IsNotNull(options);
                Assert.AreNotEqual(TransactionCrypto.unknown, options.crypto);
            }
        }

        [TestCase("1", 1, null)]
        [TestCase("1", -1, "Amount must be larger than 0")]
        [TestCase("", 1, "TokenId is invalid")]
        [TestCase("something random that isn't a token id", 1, "TokenId is invalid")]
        [TestCase("1", 0, "Amount must be larger than 0")]
        public async Task TestGetCheckoutOptions_PrimarySale_ERC721(string tokenId, long amount,
            string expectedException)
        {
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);
            ERC721Sale sale = new ERC721Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b");
            ERC721 collection = new ERC721("0xdeb398f41ccd290ee5114df7e498cf04fac916cb");

            if (!string.IsNullOrWhiteSpace(expectedException))
            {
                try
                {
                    CheckoutOptions options = await checkout.GetCheckoutOptions(sale, collection, tokenId, amount);
                    Assert.Fail("Expected exception but none was encountered");
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.Message.Contains(expectedException));
                }
            }
            else
            {
                CheckoutOptions options = await checkout.GetCheckoutOptions(sale, collection, tokenId, amount);
                Assert.IsNotNull(options);
                Assert.AreNotEqual(TransactionCrypto.unknown, options.crypto);
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGenerateBuyTransaction(int amount)
        {
            Order[] ordersResponse = await OrderFetcher.FetchListingsForCollectible("0x079294e6ffec16234578c672fa3fbfd4b6c48640", "1");
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

            Checkout checkout = new Checkout(_testWallet, _chain);

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

            Checkout checkout = new Checkout(_testWallet, _chain);

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
            Checkout checkout = new Checkout(_testWallet, _chain);
            ChainIndexer indexer = new ChainIndexer(_chain);
            Address USDC = new Address("0x750ba8b76187092B0D1E87E28daaf484d1b5273b");
            Address collection = new Address("0x079294e6ffec16234578c672fa3fbfd4b6c48640");

            GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                new GetTokenBalancesArgs(_testWallet.GetWalletAddress(), collection));
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
            Chain chain = _chain;
            Checkout checkout = new Checkout(_testWallet, chain);
            ChainIndexer indexer = new ChainIndexer(chain);
            Address erc20UniversallyMintable = new Address("0x88e57238a23e2619fd42f479d546560b44c698fe");
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");
            
            GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                new GetTokenBalancesArgs(_testWallet.GetWalletAddress(), erc20UniversallyMintable));
            Assert.IsNotNull(balancesReturn);
            TokenBalance[] balances = balancesReturn.balances;
            Assert.IsNotNull(balances);
            Assert.GreaterOrEqual(balances.Length, 1);
            Assert.GreaterOrEqual(balances[0].balance, (BigInteger)amount);

            for (int i = 0; i < amount; i++)
            {
                Step[] steps = await checkout.GenerateOfferTransaction(collection, "1",
                    1, ContractType.ERC1155, erc20UniversallyMintable,
                    1, DateTime.Now + TimeSpan.FromMinutes(30));
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }
        
        [Test]
        public async Task TestGenerateOfferTransaction_NativeCurrency()
        {
            Chain chain = _chain;
            Checkout checkout = new Checkout(_testWallet, chain);
            ChainIndexer indexer = new ChainIndexer(chain);
            Address currency = new Address(StringExtensions.ZeroAddress);
            Address collection = new Address("0x0ee3af1874789245467e7482f042ced9c5171073");

            try
            {
                Step[] steps = await checkout.GenerateOfferTransaction(collection, "1",
                    1, ContractType.ERC1155, currency,
                    1, DateTime.Now + TimeSpan.FromMinutes(30));

                Assert.Fail("Expected exception but none was encountered");
            }
            catch (Exception e)
            {
                Assert.True(e.Message.Contains("Creating an offer with native currencies is not supported. Please use an ERC20 token address"));
            }
        }

        [Test]
        public async Task TestCreateListingAndBuyIt()
        {
            EndToEndTestHarness testHarness = new EndToEndTestHarness();
            bool listingCreated = false;
            Address erc20UniversallyMintable = new Address("0x88e57238a23e2619fd42f479d546560b44c698fe");
            Address collection = new Address("0x079294e6ffec16234578c672fa3fbfd4b6c48640");
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

                ChainIndexer indexer = new ChainIndexer(_chain);
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
                TransactionReturn result = await wallet.SendTransaction(_chain, new Transaction[]
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

            TransactionReturn result = await steps.SubmitAsTransactions(wallet, Chain.Polygon);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is SuccessfulTransactionReturn);
            if (result is SuccessfulTransactionReturn success)
            {
                Debug.Log("Transaction hash from step submission: " + success.txHash);
            }
        }

        private async Task SeedWalletAndCreateListing(IWallet wallet, Address erc1155UniversallyMintable,
            Address currencyToken, int tokenId = 1)
        {
            Checkout checkout = new Checkout(wallet, _chain);
            ChainIndexer indexer = new ChainIndexer(_chain);

            ERC1155 universallyMintable = new ERC1155(erc1155UniversallyMintable);
            TransactionReturn result = await wallet.SendTransaction(_chain, new Transaction[]
            {
                new RawTransaction(erc1155UniversallyMintable, "0",
                    universallyMintable.Mint(wallet.GetWalletAddress(), tokenId, 10000).CallData)
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

            CollectibleOrder[] collectibleOrders = await FetchListings(collection, _chain);
            Assert.Greater(collectibleOrders.Length, 0);

            Checkout checkout = new Checkout(wallet, _chain);
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
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            CollectibleOrder[] collectibleOrders = null;
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                collectibleOrders = await marketplaceReader.ListAllCollectibleListingsWithLowestPricedListingsFirst(collection, filter);
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
            Chain chain = Chain.Polygon;
            bool offerCreated = false;
            Address erc20UniversallyMintable = new Address("0x88e57238a23e2619fd42f479d546560b44c698fe");
            Address collection = new Address("0x079294e6ffec16234578c672fa3fbfd4b6c48640");
            Address initialWallet = null;
            bool sold = false;

            testHarness.Login(async wallet =>
            {
                try
                {
                    initialWallet = wallet.GetWalletAddress();

                    ERC20 universallyMintable = new ERC20(erc20UniversallyMintable);
                    TransactionReturn result = await wallet.SendTransaction(chain, new Transaction[]
                    {
                        new RawTransaction(erc20UniversallyMintable, "0",
                            universallyMintable.Mint(wallet.GetWalletAddress(), 10000000).CallData)
                    });
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result is SuccessfulTransactionReturn);

                    Checkout checkout = new Checkout(wallet, chain);

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

                ChainIndexer indexer = new ChainIndexer(chain);

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
                TransactionReturn result = await wallet.SendTransaction(chain, new Transaction[]
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

            CollectibleOrder[] collectibleOrders = await FetchOffers(collection, Chain.Polygon);

            Assert.Greater(collectibleOrders.Length, 0);

            Checkout checkout = new Checkout(wallet, Chain.Polygon);
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
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            CollectibleOrder[] collectibleOrders = null;
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                collectibleOrders = await marketplaceReader.ListAllCollectibleOffersWithHighestPricedOfferFirst(collection);
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
            Address collection = new Address("0x079294e6ffec16234578c672fa3fbfd4b6c48640");
            bool cancelled = false;

            WaaSEndToEndTestConfig config = WaaSEndToEndTestConfig.GetConfig();
            
            string email = config.PlayFabEmail;
            var request = new LoginWithEmailAddressRequest { Email = email, Password = config.PlayFabPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, result =>
            {
                testHarness.LoginWithPlayFab(result, email, async wallet =>
                {
                    Order myOrder = null;
                    int retries = 5;
                    for (int i = 0; i < retries; i++)
                    {
                        CollectibleOrder[] collectibleOrders = await FetchListings(collection, _chain, 
                            new CollectiblesFilter(true, "", ordersCreatedBy: new string[] {wallet.GetWalletAddress()}));
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
                        Assert.Fail($"Failed to find order created by wallet after {retries} retries. Try running {nameof(MarketplaceSeeder.SeedMarketplace_Playfab)} and seeding the marketplace with orders from this wallet first");
                    }

                    Checkout checkout = new Checkout(wallet, _chain);
                    Step[] steps = await checkout.GenerateCancelTransaction(collection, myOrder);
                    await SubmitStepsAsTransaction(steps, wallet);

                    for (int i = 0; i < retries; i++)
                    {
                        CollectibleOrder[] collectibleOrders = await FetchListings(collection, _chain);
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
                        Assert.Fail(
                            $"Failed to see that order {myOrder.orderId} was cancelled after {retries} retries");
                    }

                    cancelled = true;
                }, (error, method, email, methods) => { Assert.Fail(error); });
            }, error =>
            {
                Assert.Fail(error.ErrorMessage);
                cancelled = true;
            });
            
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
                if (order != null && order.createdBy.ToLower() == walletAddress.Value.ToLower())
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
                            ChainIndexer indexer = new ChainIndexer(_chain);
                            GetTokenBalancesReturn balancesReturn = await indexer.GetTokenBalances(
                                new GetTokenBalancesArgs(wallet.GetWalletAddress(), collection));
                            Assert.IsNotNull(balancesReturn);
                            TokenBalance[] balances = balancesReturn.balances;
                            BigInteger balance = BigInteger.Zero;
                            if (balances.Length != 0)
                            {
                                balance = balances[0].balance;
                            }

                            EtherBalance etherBalanceReturn = await indexer.GetEtherBalance(wallet.GetWalletAddress());
                            Assert.IsNotNull(etherBalanceReturn);
                            BigInteger etherBalance = etherBalanceReturn.balanceWei;
                            Assert.Greater(etherBalance, BigInteger.Zero, $"Wallet {wallet.GetWalletAddress()} must have ether balance or test will fail");
                            
                            Order[] orders = await FetchListingsForCollectible(collection, "1", _chain, 
                                new OrderFilter(null, null, new string[] {Currency.NativeCurrencyAddress}));
                            Assert.Greater(orders.Length, 0);

                            Order order = orders[0];
                            Checkout checkout = new Checkout(wallet, _chain);
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
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            Order[] orders = null;
            for (int i = 0; i < 5; i++) // Retry up to 5 times as the listing might not have been picked up yet
            {
                orders = await marketplaceReader.ListAllListingsForCollectible(new Address(collection), tokenId, filter);
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