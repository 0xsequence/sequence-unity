using System;
using Sequence.EmbeddedWallet;

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