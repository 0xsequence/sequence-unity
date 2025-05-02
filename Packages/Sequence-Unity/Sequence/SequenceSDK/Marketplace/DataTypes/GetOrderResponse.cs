using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetOrderResponse
    {
        public CollectibleOrder collectible;

        [Preserve]
        public GetOrderResponse(CollectibleOrder collectible)
        {
            this.collectible = collectible;
        }
    }
}