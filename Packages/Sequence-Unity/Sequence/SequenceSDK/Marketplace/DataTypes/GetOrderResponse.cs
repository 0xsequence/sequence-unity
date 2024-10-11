using System;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetOrderResponse
    {
        public CollectibleOrder collectible;

        public GetOrderResponse(CollectibleOrder collectible)
        {
            this.collectible = collectible;
        }
    }
}