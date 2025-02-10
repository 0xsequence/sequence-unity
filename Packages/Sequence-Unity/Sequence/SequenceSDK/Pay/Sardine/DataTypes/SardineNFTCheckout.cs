using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public class SardineNFTCheckout
    {
        public string token;
        public string expiresAt;
        public string orderId;

        [Preserve]
        public SardineNFTCheckout(string token, string expiresAt, string orderId)
        {
            this.token = token;
            this.expiresAt = expiresAt;
            this.orderId = orderId;
        }
    }
}