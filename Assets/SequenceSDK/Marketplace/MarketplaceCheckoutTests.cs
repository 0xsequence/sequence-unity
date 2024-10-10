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
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleListingsWithLowestPricedListingsFirst(contractAddress, filter);
            Assert.IsNotNull(collectiblesResponse);
            Assert.IsNotNull(collectiblesResponse.collectibles);
            int length = collectiblesResponse.collectibles.Length;
            Assert.Greater(length, 0);
            _collectibleOrders = collectiblesResponse.collectibles;

            for (int i = 0; i < length; i++)
            {
                Assert.IsNotNull(_collectibleOrders[i]);
                Assert.IsNotNull(_collectibleOrders[i].order);
            }
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
            Assert.IsNotNull(options.swap);
            Assert.IsNotNull(options.nftCheckout);
            Assert.IsNotNull(options.onRamp);
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
    }
}