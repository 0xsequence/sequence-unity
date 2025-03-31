using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCollectibleOrderRequest
    {
        public Address contractAddress;
        public string tokenId;
        public OrderFilter filter;

        [Preserve]
        public GetCollectibleOrderRequest(Address contractAddress, string tokenId, OrderFilter filter)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
            this.filter = filter;
        }
    }
}