using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCurrenciesResponse
    {
        public Currency[] currencies;

        public ListCurrenciesResponse(Currency[] currencies)
        {
            this.currencies = currencies;
        }
    }
}