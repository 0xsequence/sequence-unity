using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCollectibleRequest
    {
        public Address contractAddress;
        public string tokenId;

        public GetCollectibleRequest(Address contractAddress, string tokenId)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
        }
    }
}