using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;

namespace Sequence.Marketplace
{
    public class Checkout
    {
        private IHttpClient _client;
        private IWallet _wallet;
        private Chain _chain;

        public Checkout(IWallet wallet, Chain chain, IHttpClient client = null)
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
            GetCheckoutOptionsRequest request = new GetCheckoutOptionsRequest(_wallet.GetWalletAddress(), orders, additionalFeeBps);
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
        
        public async Task<Step[]> GenerateBuyTransaction(Order order, AdditionalFee additionalFee = null)
        {
            OrderData[] ordersData = new OrderData[]
                { new OrderData(order.orderId, order.quantityAvailable) };
            AdditionalFee[] additionalFees = new AdditionalFee[] { additionalFee };
            if (additionalFee == null)
            {
                additionalFees = null;
            }
            GenerateBuyTransaction generateBuyTransaction = new GenerateBuyTransaction(order.collectionContractAddress, _wallet.GetWalletAddress(),
                order.marketplace, ordersData, additionalFees, _wallet.GetWalletKind());

            try
            {
                GenerateTransactionResponse response = await _client.SendRequest<GenerateBuyTransaction, GenerateTransactionResponse>(_chain, "GenerateBuyTransaction",
                    generateBuyTransaction);
                return response.steps;
            }
            catch (Exception e)
            {
                throw new Exception($"Error generating buy transaction for {_wallet} with order {order} and {nameof(ordersData)} {ordersData}: {e.Message}");
            }
        }
        
        public async Task<Step[]> GenerateSellTransaction(Order order, AdditionalFee additionalFee = null)
        {
            OrderData[] ordersData = new OrderData[]
                { new OrderData(order.orderId, order.quantityAvailable) };
            AdditionalFee[] additionalFees = new AdditionalFee[] { additionalFee };
            if (additionalFee == null)
            {
                additionalFees = null;
            }
            GenerateBuyTransaction generateBuyTransaction = new GenerateBuyTransaction(order.collectionContractAddress, _wallet.GetWalletAddress(),
                order.marketplace, ordersData, additionalFees, _wallet.GetWalletKind());

            try
            {
                GenerateTransactionResponse response = await _client.SendRequest<GenerateBuyTransaction, GenerateTransactionResponse>(_chain, "GenerateSellTransaction",
                    generateBuyTransaction);
                return response.steps;
            }
            catch (Exception e)
            {
                throw new Exception($"Error generating sell transaction for {_wallet} with order {order} and {nameof(ordersData)} {ordersData}: {e.Message}");
            }
        }
    }
}