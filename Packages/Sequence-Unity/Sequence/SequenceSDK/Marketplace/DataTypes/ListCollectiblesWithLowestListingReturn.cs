using System;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectiblesWithLowestListingReturn
    {
        public CollectibleOrder[] collectibles;
        public Page page;

        [Preserve]
        public ListCollectiblesWithLowestListingReturn(CollectibleOrder[] collectibles, Page page = null)
        {
            this.collectibles = collectibles;
            this.page = page;
        }
    }
}