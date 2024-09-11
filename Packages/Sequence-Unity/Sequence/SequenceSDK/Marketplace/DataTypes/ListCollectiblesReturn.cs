using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectiblesReturn
    {
        public CollectibleOrder[] collectibles;
        public Page page;

        public ListCollectiblesReturn(CollectibleOrder[] collectibles, Page page = null)
        {
            this.collectibles = collectibles;
            this.page = page;
        }
    }
}