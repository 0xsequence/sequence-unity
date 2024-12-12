using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetSwapQuoteResponse
    {
        public SwapQuote swapQuote;

        [Preserve]
        public GetSwapQuoteResponse(SwapQuote swapQuote)
        {
            this.swapQuote = swapQuote;
        }
    }
}