using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [System.Serializable]
    public class OrderData
    {
        public string orderId;
        public string quantity;
        public string tokenId;
        
        [Preserve]
        public OrderData(string orderId, string quantity, string tokenId = null)
        {
            this.orderId = orderId;
            this.quantity = quantity;
            this.tokenId = tokenId;
        }
    }
}