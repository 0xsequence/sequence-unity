using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCheckoutOptionsArgs
    {
        public Address wallet;
        public CheckoutOptionsMarketplaceOrder[] orders;
        public int feeBPS;

        [Preserve]
        public GetCheckoutOptionsArgs(Address wallet, CheckoutOptionsMarketplaceOrder[] orders, int feeBps)
        {
            this.wallet = wallet;
            this.orders = orders;
            feeBPS = feeBps;
        }
    }
}