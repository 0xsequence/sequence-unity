using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCurrenciesResponse
    {
        public Currency[] currencies;

        [Preserve]
        public ListCurrenciesResponse(Currency[] currencies)
        {
            this.currencies = currencies;
        }
    }
}