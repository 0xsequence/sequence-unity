using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EmbeddedWallet;

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
    }
}