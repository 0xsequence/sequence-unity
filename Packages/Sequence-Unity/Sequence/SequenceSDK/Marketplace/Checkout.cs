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
            int additionalFeeBps = 0)
        {
            GetCheckoutOptionsRequest request = new GetCheckoutOptionsRequest(_wallet, orders, additionalFeeBps);
            try
            {
                return await _client.SendRequest<GetCheckoutOptionsRequest, CheckoutOptions>(_chain,
                    "CheckoutOptionsMarketplace", request);
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching checkout options for {_wallet} with orders {orders.AsString()} and {nameof(additionalFeeBps)} {additionalFeeBps}: {e.Message}");
            }
        }

        public Task<CheckoutOptions> GetCheckoutOptions(Order[] orders, int additionalFeeBps = 0)
        {
            if (orders == null)
            {
                throw new ArgumentException($"{nameof(orders)} cannot be null");
            }
            int length = orders.Length;
            CheckoutOptionsMarketplaceOrder[] options = new CheckoutOptionsMarketplaceOrder[length];
            for (int i = 0; i < length; i++)
            {
                options[i] = new CheckoutOptionsMarketplaceOrder(new Address(orders[i].collectionContractAddress), orders[i].orderId, orders[i].marketplace);
            }

            return GetCheckoutOptions(options, additionalFeeBps);
        }
    }
}