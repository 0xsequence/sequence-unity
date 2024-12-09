using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCheckoutOptionsArgs
    {
        public Address wallet;
        public CheckoutOptionsMarketplaceOrder[] orders;
        public int additionalFee;

        [Preserve]
        public GetCheckoutOptionsArgs(Address wallet, CheckoutOptionsMarketplaceOrder[] orders, int additionalFee)
        {
            this.wallet = wallet;
            this.orders = orders;
            additionalFee = additionalFee;
        }
    }
}