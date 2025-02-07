using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SardineQuoteResponse
    {
        public SardineQuote quote;

        [Preserve]
        public SardineQuoteResponse(SardineQuote quote)
        {
            this.quote = quote;
        }
    }
}