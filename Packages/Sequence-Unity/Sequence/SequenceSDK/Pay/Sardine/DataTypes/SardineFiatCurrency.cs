using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public class SardineFiatCurrency
    {
        public string currencyCode;
        public string name;
        public string currencySymbol;
        public SardinePaymentOption[] paymentOptions;
        public string[] supportedCountries;

        [Preserve]
        public SardineFiatCurrency(string currencyCode, string name, string currencySymbol, SardinePaymentOption[] paymentOptions, string[] supportedCountries)
        {
            this.currencyCode = currencyCode;
            this.name = name;
            this.currencySymbol = currencySymbol;
            this.paymentOptions = paymentOptions;
            this.supportedCountries = supportedCountries;
        }
    }
}