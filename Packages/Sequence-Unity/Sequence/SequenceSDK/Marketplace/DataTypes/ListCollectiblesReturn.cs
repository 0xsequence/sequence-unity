using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectiblesReturn
    {
        public CollectibleOrder[] collectibles;
        public Page page;

        [Preserve]
        public ListCollectiblesReturn(CollectibleOrder[] collectibles, Page page = null)
        {
            this.collectibles = collectibles;
            this.page = page;
        }
    }
}