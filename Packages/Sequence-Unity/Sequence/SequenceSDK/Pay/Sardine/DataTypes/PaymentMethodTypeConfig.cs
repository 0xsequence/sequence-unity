using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public class PaymentMethodTypeConfig
    {
        public PaymentMethod[] enabled;
        
        [Preserve]
        [JsonProperty("default")]
        public PaymentMethod defaultPaymentMethod;

        [Preserve]
        public PaymentMethodTypeConfig(PaymentMethod[] enabled, PaymentMethod defaultPaymentMethod)
        {
            this.enabled = enabled;
            this.defaultPaymentMethod = defaultPaymentMethod;
        }
    }
}