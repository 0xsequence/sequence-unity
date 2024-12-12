using System;
using UnityEngine.Scripting;

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