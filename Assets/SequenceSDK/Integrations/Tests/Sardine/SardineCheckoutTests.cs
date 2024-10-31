using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EmbeddedWallet;
using Sequence.Integrations.Sardine;
using Sequence.Marketplace;

namespace Sequence.Integrations.Tests.Sardine
{
    public class SardineCheckoutTests
    {
        private CollectibleOrder[] _collectibleOrders;
        
        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);
        
        [SetUp]
        public async Task Setup()
        {
            _collectibleOrders = await OrderFetcher.FetchListings();
        }
        
        [Test]
        public async Task TestCheckSardineWhitelistStatus()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            bool result =
                await sardine.CheckSardineWhitelistStatus(new Address("0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb"));

            Assert.False(result);
        }

        [Test]
        public async Task TestGetSardineSupportedRegions()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);
            
            SardineRegion[] regions = await sardine.SardineGetSupportedRegions();
            
            Assert.NotNull(regions);
            Assert.Greater(regions.Length, 0);
        }

        [Test]
        public async Task TestSardineGetClientToken()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            string token = await sardine.SardineGetClientToken();
            
            Assert.NotNull(token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [Test]
        public async Task TestSardineGetNFTCheckoutToken()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            SardineNFTCheckout token = await sardine.SardineGetNFTCheckoutToken(_collectibleOrders[0], 1);
            
            Assert.NotNull(token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.token));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.expiresAt));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.orderId));
        }
    }
}