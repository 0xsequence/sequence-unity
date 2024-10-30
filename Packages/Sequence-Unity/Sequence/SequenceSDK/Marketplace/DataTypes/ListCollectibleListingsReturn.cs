using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectibleListingsReturn
    {
        public Order[] listings;
        public Page page;

        [Preserve]
        public ListCollectibleListingsReturn(Order[] listings, Page page)
        {
            this.listings = listings;
            this.page = page;
        }
    }
}