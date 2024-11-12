using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.EmbeddedWallet.Tests;

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
                    await Task.Delay(3000); // Allow some time for the indexer to pick up transaction and for transaction to finalize
            
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
            
            bool bought = false;
            testHarness.Login(async wallet =>
            {
                Assert.AreNotEqual(initialWallet, wallet.GetWalletAddress());
                await Task.Delay(10000);
                Checkout checkout = new Checkout(wallet, Chain.ArbitrumNova);
                MarketplaceReader reader = new MarketplaceReader(Chain.ArbitrumNova);
                
                ERC20 universallyMintable = new ERC20(erc20UniversallyMintable);
                TransactionReturn result = await wallet.SendTransaction(Chain.ArbitrumNova, new Transaction[]
                {
                    new RawTransaction(erc20UniversallyMintable, "0",
                        universallyMintable.Mint(wallet.GetWalletAddress(), 10000000).CallData)
                });
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SuccessfulTransactionReturn);

                _collectibleOrders = await reader.ListAllCollectibleListingsWithLowestPricedListingsFirst(collection);
                Assert.IsNotNull(_collectibleOrders);
                Assert.Greater(_collectibleOrders.Length, 0);
                
                Step[] steps = await checkout.GenerateBuyTransaction(_collectibleOrders[0].order);
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
    }
}