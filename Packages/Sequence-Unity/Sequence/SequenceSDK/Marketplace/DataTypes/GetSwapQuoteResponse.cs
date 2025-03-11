using System;
using Sequence.Utils;

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