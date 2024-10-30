using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCollectibleResponse
    {
        public TokenMetadata metadata;

        [Preserve]
        public GetCollectibleResponse(TokenMetadata metadata)
        {
            this.metadata = metadata;
        }
    }
}