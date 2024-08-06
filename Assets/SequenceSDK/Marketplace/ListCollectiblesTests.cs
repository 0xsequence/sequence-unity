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
            Chain chain = Chain.ArbitrumNova;
            ListCollectibles listCollectibles = new ListCollectibles(chain);
            string contractAddress = "0x4279aa50a32b8c892206f4ef1a25befb6fd33922";
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
            Assert.Greater(collectibles.Length, 0);
            Assert.Greater(successEvents, 0);
            Assert.AreEqual(failEvents, 0);
        }
    }
}