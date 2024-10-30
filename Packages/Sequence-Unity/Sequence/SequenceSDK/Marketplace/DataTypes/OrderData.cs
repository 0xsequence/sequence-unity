using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [System.Serializable]
    public class OrderData
    {
        public string orderId;
        public string quantity;
        
        [Preserve]
        public OrderData(string orderId, string quantity)
        {
            this.orderId = orderId;
            this.quantity = quantity;
        }
    }
}