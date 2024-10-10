using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCheckoutOptionsResponse
    {
        public CheckoutOptions options;

        public GetCheckoutOptionsResponse(CheckoutOptions options)
        {
            this.options = options;
        }
    }
}