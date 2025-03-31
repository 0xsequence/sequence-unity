using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCheckoutOptionsResponse
    {
        public CheckoutOptions options;

        [Preserve]
        public GetCheckoutOptionsResponse(CheckoutOptions options)
        {
            this.options = options;
        }
    }
}