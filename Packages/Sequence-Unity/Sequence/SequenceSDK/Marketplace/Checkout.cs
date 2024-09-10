using System;
using System.Threading.Tasks;

namespace Sequence.Marketplace
{
    public class Checkout
    {
        private IHttpClient _client;
        private Address _wallet;
        private Chain _chain;

        public Checkout(Address wallet, Chain chain, IHttpClient client = null)
        {
            _wallet = wallet;
            _chain = chain;
            if (client == null)
            {
                client = new HttpClient();
            }
            _client = client;
        }

        public async Task<CheckoutOptions> GetCheckoutOptions(CheckoutOptionsMarketplaceOrder[] orders,
            int feeBPS)
        {
            GetCheckoutOptionsRequest request = new GetCheckoutOptionsRequest(_wallet, orders, feeBPS);
            try
            {
                return await _client.SendRequest<GetCheckoutOptionsRequest, CheckoutOptions>(_chain,
                    "CheckoutOptionsMarketplace", request);
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching checkout options for {_wallet} with orders {orders.AsString()} and {nameof(feeBPS)} {feeBPS}: {e.Message}");
            }
        }
    }
}