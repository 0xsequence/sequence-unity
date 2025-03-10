using System;
using Sequence.EmbeddedWallet;

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