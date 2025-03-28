using System;

namespace Sequence
{
    [Serializable]
    public class Price
    {
        public decimal value;
        public string currency;
        
        public Price(decimal value, string currency)
        {
            this.value = value;
            this.currency = currency;
        }
    }
}