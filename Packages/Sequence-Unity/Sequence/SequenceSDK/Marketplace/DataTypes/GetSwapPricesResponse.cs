using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetSwapPricesResponse
    {
        public SwapPrice[] swapPermit2Prices;

        [Preserve]
        public GetSwapPricesResponse(SwapPrice[] swapPermit2Prices)
        {
            this.swapPermit2Prices = swapPermit2Prices;
        }
    }
}