using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectiblesWithLowestListingReturn
    {
        public CollectibleOrder[] collectibles;
        public Page page;

        public ListCollectiblesWithLowestListingReturn(CollectibleOrder[] collectibles, Page page = null)
        {
            this.collectibles = collectibles;
            this.page = page;
        }
    }
}