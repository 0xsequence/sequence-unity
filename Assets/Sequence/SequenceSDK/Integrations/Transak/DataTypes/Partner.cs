using System;

namespace Sequence.Integrations.Transak
{
    [Serializable]
    public class Partner
    {
        public string name;
        public bool isCardPayment;
        public string currencyCode;

        public Partner(string name, bool isCardPayment, string currencyCode)
        {
            this.name = name;
            this.isCardPayment = isCardPayment;
            this.currencyCode = currencyCode;
        }
    }
}