using System;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetSwapPriceResponse
    {
        public SwapPrice swapPrice;

        [Preserve]
        public GetSwapPriceResponse(SwapPrice swapPrice)
        {
            this.swapPrice = swapPrice;
        }
    }
}