using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class PaymentMethodTypeConfig
    {
        public PaymentMethod[] enabled;
        
        [JsonProperty("default")]
        public PaymentMethod defaultPaymentMethod;

        public PaymentMethodTypeConfig(PaymentMethod[] enabled, PaymentMethod defaultPaymentMethod)
        {
            this.enabled = enabled;
            this.defaultPaymentMethod = defaultPaymentMethod;
        }
    }
}