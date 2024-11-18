using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;

namespace Sequence.Marketplace
{
    public class Checkout : ICheckout
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

        public event Action<CheckoutOptions> OnCheckoutOptionsReturn;
        public event Action<string> OnCheckoutOptionsError;
        
        public async Task<CheckoutOptions> GetCheckoutOptions(CheckoutOptionsMarketplaceOrder[] orders,
            int additionalFeeBps = 0)
        {
            GetCheckoutOptionsArgs args = new GetCheckoutOptionsArgs(_wallet.GetWalletAddress(), orders, additionalFeeBps);
            try
            {
                GetCheckoutOptionsResponse response = await _client.SendRequest<GetCheckoutOptionsArgs, GetCheckoutOptionsResponse>(_chain,
                    "CheckoutOptionsMarketplace", args);
                OnCheckoutOptionsReturn?.Invoke(response.options);
                return response.options;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching checkout options for {_wallet} with orders {orders.AsString()} and {nameof(additionalFeeBps)} {additionalFeeBps}: {e.Message}";
                OnCheckoutOptionsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public Task<CheckoutOptions> GetCheckoutOptions(Order[] orders, int additionalFeeBps = 0)
        {
            if (orders == null)
            {
                string error = $"{nameof(orders)} cannot be null";
                OnCheckoutOptionsError?.Invoke(error);
                throw new ArgumentException(error);
            }
            int length = orders.Length;
            CheckoutOptionsMarketplaceOrder[] options = new CheckoutOptionsMarketplaceOrder[length];
            for (int i = 0; i < length; i++)
            {
                options[i] = new CheckoutOptionsMarketplaceOrder(new Address(orders[i].collectionContractAddress), orders[i].orderId, orders[i].marketplace);
            }

            return GetCheckoutOptions(options, additionalFeeBps);
        }
        
        public event Action<Step[]> OnTransactionStepsReturn;
        public event Action<string> OnTransactionStepsError;
        
        public async Task<Step[]> GenerateBuyTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null)
        {
            OrderData[] ordersData = new OrderData[]
                { new OrderData(order.orderId, amount.ToString()) };
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
                OnTransactionStepsReturn?.Invoke(response.steps);
                return response.steps;
            }
            catch (Exception e)
            {
                string error =
                    $"Error generating buy transaction for {_wallet} with order {order} and {nameof(ordersData)} {ordersData}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }
        
        public async Task<Step[]> GenerateSellTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null)
        {
            OrderData[] ordersData = new OrderData[]
                { new OrderData(order.orderId, amount.ToString()) };
            AdditionalFee[] additionalFees = new AdditionalFee[] { additionalFee };
            if (additionalFee == null)
            {
                additionalFees = null;
            }
            GenerateSellTransaction generateBuyTransaction = new GenerateSellTransaction(order.collectionContractAddress, _wallet.GetWalletAddress(),
                order.marketplace, ordersData, additionalFees, _wallet.GetWalletKind());

            try
            {
                GenerateTransactionResponse response = await _client.SendRequest<GenerateSellTransaction, GenerateTransactionResponse>(_chain, "GenerateSellTransaction",
                    generateBuyTransaction);
                OnTransactionStepsReturn?.Invoke(response.steps);
                return response.steps;
            }
            catch (Exception e)
            {
                string error =
                    $"Error generating sell transaction for {_wallet} with order {order} and {nameof(ordersData)} {ordersData}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public async Task<Step[]> GenerateListingTransaction(Address collection, string tokenId, BigInteger amount, ContractType contractType, Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry, OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2)
        {
            long epochTime = ((DateTimeOffset)expiry).ToUnixTimeSeconds();
            GenerateListingTransactionArgs args = new GenerateListingTransactionArgs(collection, _wallet.GetWalletAddress(), contractType, orderbookKind, 
                new CreateReq(tokenId, amount.ToString(), epochTime.ToString(), currencyTokenAddress, pricePerToken.ToString()), _wallet.GetWalletKind());

            try
            {
                GenerateTransactionResponse response = await _client.SendRequest<GenerateListingTransactionArgs, GenerateTransactionResponse>(_chain, "GenerateListingTransaction", args);
                OnTransactionStepsReturn?.Invoke(response.steps);
                return response.steps;
            }
            catch (Exception e)
            {
                string error = $"Error generating listing transaction for {_wallet} with args {args}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public async Task<Step[]> GenerateOfferTransaction(Address collection, string tokenId, BigInteger amount, ContractType contractType, Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry, OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2)
        {
            long epochTime = ((DateTimeOffset)expiry).ToUnixTimeSeconds();
            GenerateOfferTransactionArgs args = new GenerateOfferTransactionArgs(collection, _wallet.GetWalletAddress(), contractType, orderbookKind, 
                new CreateReq(tokenId, amount.ToString(), epochTime.ToString(), currencyTokenAddress, pricePerToken.ToString()), _wallet.GetWalletKind());

            try
            {
                GenerateTransactionResponse response = await _client.SendRequest<GenerateOfferTransactionArgs, GenerateTransactionResponse>(_chain, "GenerateOfferTransaction", args);
                OnTransactionStepsReturn?.Invoke(response.steps);
                return response.steps;
            }
            catch (Exception e)
            {
                string error = $"Error generating offer transaction for {_wallet} with args {args}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public async Task<Step[]> GenerateCancelTransaction(Address collection, string orderId,
            MarketplaceKind marketplaceKind = MarketplaceKind.sequence_marketplace_v2)
        {
            GenerateCancelTransactionRequest args =
                new GenerateCancelTransactionRequest(collection, _wallet.GetWalletAddress(), marketplaceKind, orderId);
            
            try
            {
                GenerateTransactionResponse response = await _client.SendRequest<GenerateCancelTransactionRequest, GenerateTransactionResponse>(_chain, "GenerateCancelTransaction", args);
                OnTransactionStepsReturn?.Invoke(response.steps);
                return response.steps;
            }
            catch (Exception e)
            {
                string error = $"Error generating cancel transaction for {_wallet} with args {args}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }
        
        public Task<Step[]> GenerateCancelTransaction(Address collection, Order order,
            MarketplaceKind marketplaceKind = MarketplaceKind.sequence_marketplace_v2)
        {
            return GenerateCancelTransaction(collection, order.orderId, marketplaceKind);
        }
    }
}