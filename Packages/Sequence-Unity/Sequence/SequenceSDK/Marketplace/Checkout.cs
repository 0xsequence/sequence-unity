using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
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
        
        public async Task<CheckoutOptions> GetCheckoutOptions(ERC1155Sale saleContract, Address collection, Dictionary<string, BigInteger> amountsByTokenId)
        {
            try
            {
                GetPrimarySaleCheckoutOptionsArgs args = new GetPrimarySaleCheckoutOptionsArgs(_wallet.GetWalletAddress(), saleContract, collection, amountsByTokenId);
                GetCheckoutOptionsResponse response = await _client.SendRequest<GetPrimarySaleCheckoutOptionsArgs, GetCheckoutOptionsResponse>(_chain,
                    "CheckoutOptionsSalesContract", args);
                OnCheckoutOptionsReturn?.Invoke(response.options);
                return response.options;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching checkout options for {_wallet} with sale contract {saleContract.Contract.GetAddress()} and {nameof(collection)} {collection}: {e.Message}";
                OnCheckoutOptionsError?.Invoke(error);
                throw new Exception(error);
            }
        }
        
        public async Task<CheckoutOptions> GetCheckoutOptions(ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount)
        {
            try
            {
                GetPrimarySaleCheckoutOptionsArgs args = new GetPrimarySaleCheckoutOptionsArgs(_wallet.GetWalletAddress(), saleContract, collection, tokenId, amount);
                GetCheckoutOptionsResponse response = await _client.SendRequest<GetPrimarySaleCheckoutOptionsArgs, GetCheckoutOptionsResponse>(_chain,
                    "CheckoutOptionsSalesContract", args);
                OnCheckoutOptionsReturn?.Invoke(response.options);
                return response.options;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching checkout options for {_wallet} with sale contract {saleContract.Contract.GetAddress()} and {nameof(collection)} {collection}: {e.Message}";
                OnCheckoutOptionsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public event Action<Step[]> OnTransactionStepsReturn;
        public event Action<string> OnTransactionStepsError;
        
        public Task<Step[]> GenerateBuyTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null, Address buyer = null, WalletKind walletType = WalletKind.unspecified)
        {
            OrderData[] ordersData = new OrderData[]
                { new OrderData(order.orderId, amount.ToString()) };
            AdditionalFee[] additionalFees = new AdditionalFee[] { additionalFee };
            if (additionalFee == null)
            {
                additionalFees = null;
            }
            
            return GenerateBuyTransaction(ordersData, order.collectionContractAddress, order.marketplace, additionalFees, buyer, walletType);
        }
        
        private async Task<Step[]> GenerateBuyTransaction(OrderData[] ordersData, string collectionContractAddress, MarketplaceKind marketplaceKind, AdditionalFee[] additionalFees = null, Address buyer = null, WalletKind walletType = WalletKind.unspecified)
        {
            if (buyer == null)
            {
                buyer = _wallet.GetWalletAddress();
            }

            if (walletType == WalletKind.unspecified)
            {
                walletType = _wallet.GetWalletKind();
            }
            GenerateBuyTransaction generateBuyTransaction = new GenerateBuyTransaction(collectionContractAddress, buyer,
                marketplaceKind, ordersData, additionalFees, walletType);

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
                    $"Error generating buy transaction for {_wallet} with {nameof(ordersData)} {ordersData}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public Task<Step[]> GenerateBuyTransaction(Order[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address buyer = null, WalletKind walletType = WalletKind.unspecified)
        {
            ValidateOrders(orders);

            orders = orders.OrderBy(order => order.priceUSD).ToArray();

            OrderData[] orderDatas = AssembleOrderDatas(orders, amount);

            return GenerateBuyTransaction(orderDatas, orders[0].collectionContractAddress, orders[0].marketplace,
                additionalFee, buyer, walletType);
        }

        private void ValidateOrders(Order[] orders)
        {
            if (orders == null || orders.Length < 1)
            {
                throw new ArgumentException($"{nameof(orders)} cannot be null or empty");
            }

            BigInteger collectibleId = orders[0].collectibleId;
            int length = orders.Length;
            for (int i = 0; i < length; i++)
            {
                if (orders[i].collectibleId != collectibleId)
                {
                    throw new ArgumentException("All orders must be for the same collectible");
                }
            }
        }

        private OrderData[] AssembleOrderDatas(Order[] orders, BigInteger amount)
        {
            BigInteger amountLeft = amount;
            List<OrderData> orderDatas = new List<OrderData>();
            int length = orders.Length;
            for (int i = 0; i < length; i++)
            {
                BigInteger available = BigInteger.Parse(orders[i].quantityAvailable);
                if (amountLeft <= available)
                {
                    orderDatas.Add(new OrderData(orders[i].orderId, amountLeft.ToString()));
                    amountLeft = BigInteger.Zero;
                    break;
                }
                else
                {
                    orderDatas.Add(new OrderData(orders[i].orderId, orders[i].quantityAvailable));
                    amountLeft -= available;
                    if (amountLeft == 0)
                    {
                        break;
                    }
                }
            }
            Assert.AreEqual(BigInteger.Zero, amountLeft);

            return orderDatas.ToArray();
        }

        public Task<Step[]> GenerateSellTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null, 
            Address seller = null)
        {
            OrderData[] ordersData = new OrderData[]
                { new OrderData(order.orderId, amount.ToString()) };
            AdditionalFee[] additionalFees = new AdditionalFee[] { additionalFee };
            if (additionalFee == null)
            {
                additionalFees = null;
            }
            
            return GenerateSellTransaction(ordersData, order.collectionContractAddress, order.marketplace, additionalFees, seller);
        }

        private async Task<Step[]> GenerateSellTransaction(OrderData[] ordersData, string collectionContractAddress,
            MarketplaceKind marketplaceKind, AdditionalFee[] additionalFees = null, Address seller = null)
        {
            if (seller == null)
            {
                seller = _wallet.GetWalletAddress();
            }
            
            GenerateSellTransaction generateBuyTransaction = new GenerateSellTransaction(collectionContractAddress, seller,
                marketplaceKind, ordersData, additionalFees, _wallet.GetWalletKind());

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
                    $"Error generating sell transaction for {_wallet} with {nameof(ordersData)} {ordersData}: {e.Message}";
                OnTransactionStepsError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public Task<Step[]> GenerateSellTransaction(Order[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address seller = null)
        {
            ValidateOrders(orders);

            orders = orders.OrderByDescending(order => order.priceUSD).ToArray();

            OrderData[] orderDatas = AssembleOrderDatas(orders, amount);

            return GenerateSellTransaction(orderDatas, orders[0].collectionContractAddress, orders[0].marketplace,
                additionalFee, seller);
        }

        public Task<Step[]> GenerateBuyTransaction(CollectibleOrder[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address buyer = null, WalletKind walletType = WalletKind.unspecified)
        {
            int length = orders.Length;
            Order[] orderArray = new Order[length];
            for (int i = 0; i < length; i++)
            {
                orderArray[i] = orders[i].order;
            }

            return GenerateBuyTransaction(orderArray, amount, additionalFee, buyer, walletType);
        }

        public Task<Step[]> GenerateSellTransaction(CollectibleOrder[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address seller = null)
        {
            int length = orders.Length;
            Order[] orderArray = new Order[length];
            for (int i = 0; i < length; i++)
            {
                orderArray[i] = orders[i].order;
            }

            return GenerateSellTransaction(orderArray, amount, additionalFee, seller);
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
                if (currencyTokenAddress.IsZeroAddress())
                {
                    throw new ArgumentException("Creating an offer with native currencies is not supported. Please use an ERC20 token address.");
                }
                
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