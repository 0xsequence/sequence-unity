using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    internal class CheckoutTokenResponse
    {
        public SardineNFTCheckout resp;

        [Preserve]
        public CheckoutTokenResponse(SardineNFTCheckout resp)
        {
            this.resp = resp;
        }
    }
}