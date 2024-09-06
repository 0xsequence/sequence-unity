using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineNFTCheckout
    {
        public string token;
        public string expiresAt;
        public string orderId;

        public SardineNFTCheckout(string token, string expiresAt, string orderId)
        {
            this.token = token;
            this.expiresAt = expiresAt;
            this.orderId = orderId;
        }
    }
}