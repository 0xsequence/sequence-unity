using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public static class OrderFetcher
    {
        public static async Task<CollectibleOrder[]> FetchListings()
        {
            Chain chain = Chain.ArbitrumNova;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x0ee3af1874789245467e7482f042ced9c5171073";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleListingsWithLowestPricedListingsFirst(contractAddress, filter);
            
            ValidateCollectiblesResponse(collectiblesResponse);
            return collectiblesResponse.collectibles;
        }
        
        public static async Task<CollectibleOrder[]> FetchOffers()
        {
            Chain chain = Chain.ArbitrumNova;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x0ee3af1874789245467e7482f042ced9c5171073";
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