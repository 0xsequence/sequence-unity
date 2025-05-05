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

        public static implicit operator Order(CollectibleOrder order)
        {
            return order.order;
        }
    }
    
    public static class CollectibleOrderExtensions
    {
        public static Order[] ToOrderArray(this CollectibleOrder[] collectibleOrders)
        {
            if (collectibleOrders == null)
            {
                return null;
            }
            Order[] orders = new Order[collectibleOrders.Length];
            for (int i = 0; i < collectibleOrders.Length; i++)
            {
                orders[i] = collectibleOrders[i].order;
            }

            return orders;
        }
        
        public static CollectibleOrder GetCollectibleOrder(this CollectibleOrder[] collectibleOrders, string orderId)
        {
            foreach (CollectibleOrder collectibleOrder in collectibleOrders)
            {
                if (collectibleOrder.order.orderId == orderId)
                {
                    return collectibleOrder;
                }
            }

            return null;
        }

        public static bool CheckSameCollectible(this CollectibleOrder[] orders)
        {
            int length = orders.Length;
            if (length == 0)
            {
                return true;
            }
            
            Collectible collectible = new Collectible(new Address(orders[0].order.collectionContractAddress), orders[0].order.tokenId);
            for (int i = 1; i < length; i++)
            {
                if (orders[i].order.collectionContractAddress != collectible.Contract || orders[i].order.tokenId != collectible.TokenId)
                {
                    return false;
                }
            }
            return true;
        }

        private class Collectible
        {
            public Address Contract;
            public string TokenId;

            public Collectible(Address contract, string tokenId)
            {
                Contract = contract;
                TokenId = tokenId;
            }
        }
    }
}