using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCollectibleOrderRequest
    {
        public Address contractAddress;
        public string tokenId;
        public OrderFilter filter;

        public GetCollectibleOrderRequest(Address contractAddress, string tokenId, OrderFilter filter)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
            this.filter = filter;
        }
    }
}