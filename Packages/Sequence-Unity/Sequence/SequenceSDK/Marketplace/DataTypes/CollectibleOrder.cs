using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CollectibleOrder
    {
        public TokenMetadata metadata;
        public Order order;

        [Preserve]
        public CollectibleOrder(TokenMetadata metadata, Order order = null)
        {
            this.metadata = metadata;
            this.order = order;
        }
    }
}