using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.Marketplace;

namespace SequenceSDK.Marketplace
{
    public class ListCollectiblesTests
    {
        [Test]
        public async Task TestListAllCollectiblesWithLowestListing()
        {
            Chain chain = Chain.Polygon;
            ListCollectibles listCollectibles = new ListCollectibles(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            int successEvents = 0;
            int failEvents = 0;
            listCollectibles.OnListCollectiblesWithLowestListingReturn += (ListCollectiblesWithLowestListingReturn result) =>
            {
                successEvents++;
            };
            listCollectibles.OnListCollectiblesWithLowestListingError += (string error) =>
            {
                Assert.Fail(error);
            };
            
            CollectibleOrder[] collectibles = await listCollectibles.ListAllCollectibleWithLowestListing(contractAddress, filter);
            
            Assert.IsNotNull(collectibles);
            Assert.Greater(successEvents, 0);
            Assert.AreEqual(failEvents, 0);
            Assert.Greater(collectibles.Length, 0);
        }
    }
}