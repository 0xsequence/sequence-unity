using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
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