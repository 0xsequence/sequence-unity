using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public static class OrderFetcher
    {
        public static async Task<CollectibleOrder[]> FetchListings(Chain chain, string contractAddress)
        {
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleListingsWithLowestPricedListingsFirst(contractAddress, filter);
            
            ValidateCollectiblesResponse(collectiblesResponse);
            return collectiblesResponse.collectibles;
        }

        public static async Task<Order[]> FetchListingsForCollectible(string collectionAddress, string tokenId)
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            
            ListCollectibleListingsReturn collectiblesResponse = await marketplaceReader.ListListingsForCollectible(new Address(collectionAddress), tokenId);
            
            ValidateCollectibleListingsResponse(collectiblesResponse);
            return collectiblesResponse.listings;
        }
        
        public static async Task<CollectibleOrder[]> FetchOffers()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x079294e6ffec16234578c672fa3fbfd4b6c48640";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleOffersWithHighestPricedOfferFirst(contractAddress, filter);
            
            ValidateCollectiblesResponse(collectiblesResponse);
            return collectiblesResponse.collectibles;
        }

        public static async Task<Order[]> FetchOffersForCollectible(string tokenId)
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x079294e6ffec16234578c672fa3fbfd4b6c48640";
            
            ListCollectibleOffersReturn collectiblesResponse = await marketplaceReader.ListOffersForCollectible(new Address(contractAddress), tokenId);
            
            ValidateCollectibleOffersResponse(collectiblesResponse);
            return collectiblesResponse.offers;
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

        private static void ValidateCollectibleListingsResponse(ListCollectibleListingsReturn response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.listings);
            int length = response.listings.Length;
            Assert.Greater(length, 0);
            Order[] orders = response.listings;

            for (int i = 0; i < length; i++)
            {
                Assert.IsNotNull(orders[i]);
            }
        }

        private static void ValidateCollectibleOffersResponse(ListCollectibleOffersReturn response)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.offers);
            int length = response.offers.Length;
            Assert.Greater(length, 0);
            Order[] orders = response.offers;

            for (int i = 0; i < length; i++)
            {
                Assert.IsNotNull(orders[i]);
            }
        }
    }
}