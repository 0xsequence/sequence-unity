using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetCheckoutOptionsRequest
    {
        public Address wallet;
        public CheckoutOptionsMarketplaceOrder[] orders;
        public int feeBPS;

        public GetCheckoutOptionsRequest(Address wallet, CheckoutOptionsMarketplaceOrder[] orders, int feeBps)
        {
            this.wallet = wallet;
            this.orders = orders;
            feeBPS = feeBps;
        }
    }
}