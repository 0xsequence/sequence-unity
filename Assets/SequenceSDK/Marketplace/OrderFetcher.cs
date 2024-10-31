using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public static class OrderFetcher
    {
        public static async Task<CollectibleOrder[]> FetchListings()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleListingsWithLowestPricedListingsFirst(contractAddress, filter);
            
            ValidateCollectiblesResponse(collectiblesResponse);
            return collectiblesResponse.collectibles;
        }
        
        public static async Task<CollectibleOrder[]> FetchOffers()
        {
            Chain chain = Chain.TestnetPolygonAmoy;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x5e4bfd71236a21299d43f508dbb76cb7d0fd4e50";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleOffersWithHighestPricedOfferFirst(contractAddress, filter);
            
            ValidateCollectiblesResponse(collectiblesResponse);
            return collectiblesResponse.collectibles;
        }

        private static void ValidateCollectiblesResponse(ListCollectiblesReturn response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.collectibles);
            int length = response.collectibles.Length;
            Assert.Greater(length, 0);
            CollectibleOrder[] collectibleOrders = response.collectibles;

            for (int i = 0; i < length; i++)
            {
                Assert.IsNotNull(collectibleOrders[i]);
                Assert.IsNotNull(collectibleOrders[i].order);
            }
        }
    }
}