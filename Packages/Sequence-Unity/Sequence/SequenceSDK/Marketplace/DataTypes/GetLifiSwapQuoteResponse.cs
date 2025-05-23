using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetLifiSwapQuoteResponse
    {
        public LifiSwapQuote quote;

        [Preserve]
        public GetLifiSwapQuoteResponse(LifiSwapQuote quote)
        {
            this.quote = quote;
        }
    }
}