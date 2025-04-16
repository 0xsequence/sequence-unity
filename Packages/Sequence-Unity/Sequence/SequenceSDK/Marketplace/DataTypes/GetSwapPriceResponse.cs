using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetSwapPriceResponse
    {
        public SwapPrice swapPermit2Price;

        [Preserve]
        public GetSwapPriceResponse(SwapPrice swapPermit2Price)
        {
            this.swapPermit2Price = swapPermit2Price;
        }
    }
}