using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectibleListingsReturn
    {
        public Order[] listings;
        public Page page;

        public ListCollectibleListingsReturn(Order[] listings, Page page)
        {
            this.listings = listings;
            this.page = page;
        }
    }
}