using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class OrderResponse
    {
        public Order order;
        
        public OrderResponse(Order order)
        {
            this.order = order;
        }
    }
}