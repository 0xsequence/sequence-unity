using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCollectibleRequest
    {
        public Address contractAddress;
        public string tokenId;

        [Preserve]
        public GetCollectibleRequest(Address contractAddress, string tokenId)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
        }
    }
}