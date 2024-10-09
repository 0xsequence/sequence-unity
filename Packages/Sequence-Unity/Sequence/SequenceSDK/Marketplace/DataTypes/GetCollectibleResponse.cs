using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCollectibleResponse
    {
        public TokenMetadata metadata;

        public GetCollectibleResponse(TokenMetadata metadata)
        {
            this.metadata = metadata;
        }
    }
}