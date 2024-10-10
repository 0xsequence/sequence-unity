using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCheckoutOptionsArgs
    {
        public Address wallet;
        public CheckoutOptionsMarketplaceOrder[] orders;
        public int feeBPS;

        public GetCheckoutOptionsArgs(Address wallet, CheckoutOptionsMarketplaceOrder[] orders, int feeBps)
        {
            this.wallet = wallet;
            this.orders = orders;
            feeBPS = feeBps;
        }
    }
}