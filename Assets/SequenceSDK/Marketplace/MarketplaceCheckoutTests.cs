using System;
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

        [SetUp]
        public async Task Setup()
        {
            _collectibleOrders = await OrderFetcher.FetchOrders();
        }
        
        [TestCase(new[] {0})]
        [TestCase(new [] {0, 1, 2})]
        public async Task TestGetCheckoutOptions(int[] indices)
        {
            Order[] orders = new Order[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                orders[i] = _collectibleOrders[indices[i]].order;
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            CheckoutOptions options = await checkout.GetCheckoutOptions(orders);
            
            Assert.IsNotNull(options);
            Assert.AreNotEqual(TransactionCrypto.unknown, options.crypto);
        }

        [TestCase(new[] { 0 })]
        [TestCase(new[] { 0, 1, 2 })]
        public async Task TestGenerateBuyTransaction(int[] indices)
        {
            Order[] orders = new Order[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                orders[i] = _collectibleOrders[indices[i]].order;
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            for (int i = 0; i < indices.Length; i++)
            {
                Step[] steps = await checkout.GenerateBuyTransaction(orders[i]);
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(new[] { 0 })]
        [TestCase(new[] { 0, 1, 2 })]
        public async Task TestGenerateSellTransaction(int[] indices)
        {
            Order[] orders = new Order[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                orders[i] = _collectibleOrders[indices[i]].order;
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            for (int i = 0; i < indices.Length; i++)
            {
                Step[] steps = await checkout.GenerateSellTransaction(orders[i]);
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(new[] { 0 })]
        [TestCase(new[] { 0, 1, 2 })]
        public async Task TestGenerateListingTransaction(int[] indices)
        {
            Order[] orders = new Order[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                orders[i] = _collectibleOrders[indices[i]].order;
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            for (int i = 0; i < indices.Length; i++)
            {
                Step[] steps = await checkout.GenerateListingTransaction(new Address(orders[i].collectionContractAddress), orders[i].tokenId, 
                    BigInteger.Parse(orders[i].quantityAvailable), ContractType.ERC1155, new Address(orders[i].priceCurrencyAddress), 
                    1, DateTime.Now + TimeSpan.FromMinutes(30));
                Assert.IsNotNull(steps);
                Assert.Greater(steps.Length, 0);
            }
        }

        [TestCase(new[] { 0 })]
        [TestCase(new[] { 0, 1, 2 })]
        public async Task TestGenerateOfferTransaction(int[] indices)
        {
            Order[] orders = new Order[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                orders[i] = _collectibleOrders[indices[i]].order;
            }
            Checkout checkout = new Checkout(_testWallet, Chain.Polygon);

            for (int i = 0; i < indices.Length; i++)
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