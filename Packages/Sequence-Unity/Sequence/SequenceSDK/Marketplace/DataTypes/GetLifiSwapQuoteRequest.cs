using System;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetLifiSwapQuoteRequest
    {
        public GetLifiSwapQuoteParams @params;

        public GetLifiSwapQuoteRequest(GetLifiSwapQuoteParams @params)
        {
            this.@params = @params;
        }
    }
}