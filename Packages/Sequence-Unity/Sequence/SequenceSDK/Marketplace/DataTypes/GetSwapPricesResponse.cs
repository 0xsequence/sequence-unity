using System;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetSwapPricesResponse
    {
        public SwapPrice[] swapPrices;

        [Preserve]
        public GetSwapPricesResponse(SwapPrice[] swapPrices)
        {
            this.swapPrices = swapPrices;
        }
    }
}