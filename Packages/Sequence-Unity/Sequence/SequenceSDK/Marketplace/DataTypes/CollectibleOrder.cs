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
        public CollectibleOrder(TokenMetadata metadata, Order order)
        {
            this.metadata = metadata;
            this.order = order;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            CollectibleOrder other = (CollectibleOrder) obj;
            return Equals(metadata, other.metadata) && Equals(order, other.order);
        }
    }
    
    public static class CollectibleOrderExtensions
    {
        public static Order[] ToOrderArray(this CollectibleOrder[] collectibleOrders)
        {
            Order[] orders = new Order[collectibleOrders.Length];
            for (int i = 0; i < collectibleOrders.Length; i++)
            {
                orders[i] = collectibleOrders[i].order;
            }

            return orders;
        }
    }
}