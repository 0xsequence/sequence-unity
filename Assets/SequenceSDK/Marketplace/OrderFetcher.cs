using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public static class OrderFetcher
    {
        public static async Task<CollectibleOrder[]> FetchOrders()
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
            CollectibleOrder[] collectibleOrders = collectiblesResponse.collectibles;

            for (int i = 0; i < length; i++)
            {
                Assert.IsNotNull(collectibleOrders[i]);
                Assert.IsNotNull(collectibleOrders[i].order);
            }

            return collectibleOrders;
        }
    }
}