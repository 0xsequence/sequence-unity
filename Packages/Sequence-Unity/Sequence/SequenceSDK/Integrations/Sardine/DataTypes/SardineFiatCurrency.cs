using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SardineFiatCurrency
    {
        public string currencyCode;
        public string name;
        public string currencySymbol;
        public SardinePaymentOption[] paymentOptions;
        public string[] supportedCountries;

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