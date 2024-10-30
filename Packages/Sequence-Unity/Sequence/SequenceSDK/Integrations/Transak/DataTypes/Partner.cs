using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Transak
{
    [Serializable]
    public class Partner
    {
        public string name;
        public bool isCardPayment;
        public string currencyCode;

        [Preserve]
        public Partner(string name, bool isCardPayment, string currencyCode)
        {
            this.name = name;
            this.isCardPayment = isCardPayment;
            this.currencyCode = currencyCode;
        }
    }
}