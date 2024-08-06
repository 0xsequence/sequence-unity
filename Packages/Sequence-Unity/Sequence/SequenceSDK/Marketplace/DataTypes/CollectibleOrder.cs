using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CollectibleOrder
    {
        public TokenMetadata metadata;
        public Order order;

        public CollectibleOrder(TokenMetadata metadata, Order order = null)
        {
            this.metadata = metadata;
            this.order = order;
        }
    }
}