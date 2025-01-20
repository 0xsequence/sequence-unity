using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SupportedFiatCurrenciesResponse
    {
        public SardineFiatCurrency[] tokens;

        [Preserve]
        public SupportedFiatCurrenciesResponse(SardineFiatCurrency[] tokens)
        {
            this.tokens = tokens;
        }
    }
}