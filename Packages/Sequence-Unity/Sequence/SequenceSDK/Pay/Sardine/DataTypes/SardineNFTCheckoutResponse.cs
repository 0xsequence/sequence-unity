using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    internal class SardineNFTCheckoutResponse
    {
        public SardineNFTCheckout resp;

        [Preserve]
        public SardineNFTCheckoutResponse(SardineNFTCheckout resp)
        {
            this.resp = resp;
        }
    }
}