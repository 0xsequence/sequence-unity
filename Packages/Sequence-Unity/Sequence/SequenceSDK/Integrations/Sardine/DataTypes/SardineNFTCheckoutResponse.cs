using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SardineNFTCheckoutResponse
    {
        public SardineNFTCheckout resp;

        public SardineNFTCheckoutResponse(SardineNFTCheckout resp)
        {
            this.resp = resp;
        }
    }
}