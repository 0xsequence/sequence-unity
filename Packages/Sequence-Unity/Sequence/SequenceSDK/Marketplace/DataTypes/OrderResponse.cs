using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class OrderResponse
    {
        public Order order;
        
        [Preserve]
        public OrderResponse(Order order)
        {
            this.order = order;
        }
    }
}