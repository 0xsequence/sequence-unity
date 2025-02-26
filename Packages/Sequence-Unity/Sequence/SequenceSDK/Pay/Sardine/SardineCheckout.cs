using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Config;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Pay.Sardine
{
    public class SardineCheckout : ISardineCheckout
    {
        private Chain _chain;
        private string _apiKey;
        private HttpClient _client;
        private IWallet _wallet;
        private ICheckout _checkout;
        private IMarketplaceReader _reader;
        private IEthClient _ethClient;

        private string _baseUrl;
        private const string _devUrl = "https://dev-api.sequence.app/rpc/API";
        private const string _prodUrl = "https://api.sequence.app/rpc/API";

        private const string _sardineCheckoutUrl =
            "https://sardine-checkout.sequence.info?api_url=https://sardine-api.sequence.info&client_token=";

        private const string _sardineSandboxCheckoutUrl =
            "https://sardine-checkout-sandbox.sequence.info?api_url=https://sardine-api-sandbox.sequence.info&client_token=";

        private const string _sardineCheckoutUrlSuffix = "&show_features=true";

        public SardineCheckout(Chain chain, IWallet wallet, ICheckout checkout = null, IMarketplaceReader reader = null,
            IEthClient ethClient = null)
        {
            _chain = chain;
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.Stack);
            _apiKey = config.BuilderAPIKey;
            
#if SEQUENCE_DEV_STACK || SEQUENCE_DEV
            _baseUrl = _devUrl;
#else
            _baseUrl = _prodUrl;
#endif

            _client = new HttpClient(_apiKey);
            _wallet = wallet;
            if (checkout == null)
            {
                checkout = new Checkout(wallet, chain);
            }

            _checkout = checkout;
            if (reader == null)
            {
                reader = new MarketplaceReader(chain);
            }

            _reader = reader;
            if (ethClient == null)
            {
                ethClient = new SequenceEthClient(chain);
            }

            _ethClient = ethClient;
        }

        public async Task<bool> CheckSardineWhitelistStatus(Address marketplaceAddress)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetNFTCheckoutToken";
            string referenceId = "sequence-unity-sardine-whitelist-check";
            string name = "whitelist-check";
            string imageUrl = "https://www.sequence.market/images/placeholder.png";
            string platform = "calldata_execution";
            string executionType = "smart_contract";
            string blockchainNftId = "42";
            uint quantity = 1;
            uint decimals = 0;
            string tokenAmount = "1000000";
            Address tokenAddress = new Address("0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48");
            string tokenSymbol = "USDC";
            uint tokenDecimals = 6;
            string callData = "0x1";
            GetSardineNFTCheckoutTokenRequest request = new GetSardineNFTCheckoutTokenRequest(
                new PaymentMethodTypeConfig(EnumExtensions.GetEnumValuesAsList<PaymentMethod>().ToArray(),
                    PaymentMethod.us_debit),
                imageUrl, _chain, Address.ZeroAddress, marketplaceAddress, blockchainNftId, quantity, decimals,
                tokenAmount,
                tokenAddress, tokenSymbol, tokenDecimals, callData, name, platform, executionType);

            try
            {
                await _client.SendRequest<GetSardineNFTCheckoutTokenRequest, SardineNFTCheckout>(url, request);
                return true;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("It must me allow listed") || e.Message.Contains("It must be allow listed"))
                {
                    return false;
                }

                Debug.LogWarning("Error fetching Sardine whitelist status: " + e.Message +
                                 "\nThe contract has most likely been whitelisted as we didn't receive an error indicating otherwise");
                return true;
            }
        }

        public async Task<SardineQuote> SardineGetQuote(SardineToken token, ulong amount,
            SardinePaymentType paymentType = SardinePaymentType.credit, SardineFiatCurrency quotedCurrency = null,
            SardineQuoteType quoteType = SardineQuoteType.buy)
        {
            SardineGetQuoteParams request = new SardineGetQuoteParams(token.assetSymbol, token.network, amount,
                _wallet.GetWalletAddress(), quotedCurrency?.currencySymbol, paymentType, quoteType);
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetQuote";
            try
            {
                SardineQuoteResponse response =
                    await _client.SendRequest<SardineGetQuoteParams, SardineQuoteResponse>(url, request);
                return response.quote;
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching Sardine quote: " + e.Message);
            }
        }

        public async Task<string> SardineGetClientToken()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetClientToken";
            try
            {
                SardineTokenResponse response = await _client.SendRequest<SardineTokenResponse>(url);
                return response.token;
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching Sardine client token: " + e.Message);
            }
        }

        public void OnRamp(string clientToken)
        {
            string url = CheckoutUrl(clientToken);
            Application.OpenURL(url);
        }

        public async Task OnRampAsync()
        {
            string token = await SardineGetClientToken();
            OnRamp(token);
        }

        private async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(CollectibleOrder[] orders, Address recipient,
            BigInteger quantity, Step step, Address marketplaceContractAddress)
        {
            CollectibleOrder order = orders[0];
            string priceSymbol = ChainDictionaries.GasCurrencyOf[_chain];
            string currencyAddress = order.order.priceCurrencyAddress;
            if (!currencyAddress.IsZeroAddress())
            {
                priceSymbol = await new ERC20(order.order.priceCurrencyAddress).Symbol(new SequenceEthClient(_chain));
            }

            GetSardineNFTCheckoutTokenRequest request = new GetSardineNFTCheckoutTokenRequest(
                paymentMethodTypeConfig: new PaymentMethodTypeConfig(EnumExtensions.GetEnumValuesAsList<PaymentMethod>().ToArray(),
                    PaymentMethod.us_debit),
                imageUrl: order.metadata.image, 
                network: _chain, 
                recipientAddress: recipient,
                contractAddress: marketplaceContractAddress, 
                blockchainNftId: order.order.tokenId, 
                quantity: quantity,
                decimals: order.order.quantityDecimals,
                tokenAmount: order.order.priceAmount,
                tokenAddress: new Address(order.order.priceCurrencyAddress), 
                tokenSymbol: priceSymbol, 
                tokenDecimals: order.order.priceDecimals, 
                callData: step.data,
                name: order.metadata.name);
            return await SardineGetNFTCheckoutToken(request);
        }

        private async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(GetSardineNFTCheckoutTokenRequest request)
        {
            if (request.tokenAddress.IsZeroAddress())
            {
                throw new ArgumentException(
                    "Sardine doesn't support native currency checkout; please choose a different payment token");
            }

            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetNFTCheckoutToken";
            try
            {
                CheckoutTokenResponse response =
                    await _client.SendRequest<GetSardineNFTCheckoutTokenRequest, CheckoutTokenResponse>(url, request);
                return response.resp;
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching Sardine NFT checkout token: " + e.Message);
            }
        }

        public async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(CollectibleOrder[] orders, BigInteger quantity,
            Address recipient = null, AdditionalFee[] additionalFee = null,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract)
        {
            if (recipient == null)
            {
                recipient = _wallet.GetWalletAddress();
            }

            if (orders == null || orders.Length < 1)
            {
                throw new ArgumentException($"{orders} must not be null or empty");
            }

            if (orders[0].order.priceCurrencyAddress.IsZeroAddress())
            {
                throw new ArgumentException("Sardine checkout does not support native currency checkout; please choose an order with a different payment token");
            }
            
            Step[] steps = await _checkout.GenerateBuyTransaction(orders, quantity, additionalFee, recipient);
            
            return await SardineGetNFTCheckoutToken(orders, recipient, quantity, steps.ExtractBuyStep(), new Address(marketplaceContractAddress));
        }

        // Todo add test
        public async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(ERC721Sale saleContract, Address collection,
            BigInteger amount, BigInteger tokenId, Address recipient = null, FixedByte[] proof = null)
        {
            if (recipient == null)
            {
                recipient = _wallet.GetWalletAddress();
            }
            
            if (amount <= 0)
            {
                throw new ArgumentException($"{nameof(amount)} must be greater than 0");
            }
            
            TokenMetadata metadata = await _reader.GetCollectible(collection, tokenId.ToString());
            ERC721Sale.SaleDetails saleDetails = await saleContract.GetSaleDetailsAsync(_ethClient);
            Address paymentToken = saleDetails.PaymentToken;

            if (paymentToken.IsZeroAddress())
            {
                throw new ArgumentException(
                    "Sardine checkout does not support native currency checkout; please choose an sales contract with a different payment token");
            }
            
            ERC20 paymentTokenContract = new ERC20(paymentToken);
            string paymentTokenSymbol = await paymentTokenContract.Symbol(_ethClient);
            BigInteger paymentTokenDecimals = await paymentTokenContract.Decimals(_ethClient);
            
            if (amount > saleDetails.SupplyCap)
            {
                Debug.LogWarning($"Requested amount exceeds the supply cap; requested: {amount}, supply cap: {saleDetails.SupplyCap}. Requesting the supply cap instead");
                amount = saleDetails.SupplyCap;
            }
            
            if (saleDetails.StartTimeLong > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                throw new Exception($"This collection {collection} is not yet available for sale");
            }
            if (saleDetails.EndTimeLong < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                throw new Exception($"This collection {collection} is no longer available for sale");
            }
            
            string callData = saleContract.Mint(recipient, amount, paymentToken,
                saleDetails.Cost * amount, proof).CallData;
            
            GetSardineNFTCheckoutTokenRequest request = new GetSardineNFTCheckoutTokenRequest(
                new PaymentMethodTypeConfig(
                    EnumExtensions.GetEnumValuesAsList<PaymentMethod>().ToArray(), PaymentMethod.us_debit),
                metadata.image, _chain, recipient, saleContract.Contract.GetAddress(),
                tokenId.ToString(), amount, metadata.decimals, saleDetails.Cost.ToString(), paymentToken,
                paymentTokenSymbol, paymentTokenDecimals, callData, metadata.name);
        
            return await SardineGetNFTCheckoutToken(request);
        }

        public async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(ERC1155Sale saleContract, Address collection,
            BigInteger tokenId, BigInteger amount, Address recipient = null, byte[] data = null, FixedByte[] proof = null)
        {
            if (recipient == null)
            {
                recipient = _wallet.GetWalletAddress();
            }
            
            if (amount <= 0)
            {
                throw new ArgumentException($"{nameof(amount)} must be greater than 0");
            }

            Address paymentToken = await saleContract.GetPaymentTokenAsync(_ethClient);

            if (paymentToken.IsZeroAddress())
            {
                throw new ArgumentException(
                    "Sardine checkout does not support native currency checkout; please choose a sales contract with a different payment token");
            }
            
            (string, BigInteger) paymentTokenDetails = await GetPaymentTokenDetails(paymentToken);
            string paymentTokenSymbol = paymentTokenDetails.Item1;
            BigInteger paymentTokenDecimals = paymentTokenDetails.Item2;
            TokenMetadata metadata = await _reader.GetCollectible(collection, tokenId.ToString());
            ERC1155Sale.SaleDetails saleDetails = await saleContract.TokenSaleDetailsAsync(_ethClient, tokenId);
            
            if (amount > saleDetails.SupplyCap)
            {
                Debug.LogWarning($"Requested amount exceeds the supply cap; requested: {amount}, supply cap: {saleDetails.SupplyCap}. Requesting the supply cap instead");
                amount = saleDetails.SupplyCap;
            }
            
            if (saleDetails.StartTimeLong > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                throw new Exception($"Token id {tokenId} is not yet available for sale");
            }
            if (saleDetails.EndTimeLong < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                throw new Exception($"Token id {tokenId} is no longer available for sale");
            }
            
            string callData = saleContract.Mint(recipient, new[] { tokenId }, new[] { amount }, data, paymentToken,
                saleDetails.Cost * amount, proof).CallData;
            
            GetSardineNFTCheckoutTokenRequest request = new GetSardineNFTCheckoutTokenRequest(
                new PaymentMethodTypeConfig(
                    EnumExtensions.GetEnumValuesAsList<PaymentMethod>().ToArray(), PaymentMethod.us_debit),
                metadata.image, _chain, recipient, saleContract,
                tokenId.ToString(), amount, metadata.decimals, saleDetails.Cost.ToString(), paymentToken,
                paymentTokenSymbol, paymentTokenDecimals, callData, metadata.name);

            return await SardineGetNFTCheckoutToken(request);
        }
        
        private async Task<(string, BigInteger)> GetPaymentTokenDetails(Address paymentToken)
        {
            string paymentTokenSymbol = "";
            BigInteger paymentTokenDecimals = -1;
            if (paymentToken.Value == Address.ZeroAddress.Value)
            {
                paymentTokenSymbol = ChainDictionaries.GasCurrencyOf[_chain];
                paymentTokenDecimals = 18;
            }
            else
            {
                ERC20 paymentTokenContract = new ERC20(paymentToken);
                paymentTokenSymbol = await paymentTokenContract.Symbol(_ethClient);
                paymentTokenDecimals = await paymentTokenContract.Decimals(_ethClient);
            }

            return (paymentTokenSymbol, paymentTokenDecimals);
        }

        public async Task<SardineOrder> SardineGetNFTCheckoutOrderStatus(string orderId)
        {
            GetOrderStatusRequest request = new GetOrderStatusRequest(orderId);
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetNFTCheckoutOrderStatus";
            try {
                SardineOrderResponse response = await _client.SendRequest<GetOrderStatusRequest, SardineOrderResponse>(url, request);
                return response.resp;
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine NFT checkout order status: " + e.Message);
            }
        }

        public async Task<SardineRegion[]> SardineGetSupportedRegions()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetSupportedRegions";
            try {
                SardineSupportedRegionsResponse response = await _client.SendRequest<SardineSupportedRegionsResponse>(url);
                return response.regions;
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine supported regions: " + e.Message);
            }
        }

        public async Task<SardineFiatCurrency[]> SardineGetSupportedFiatCurrencies()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetSupportedFiatCurrencies";
            try
            {
                SupportedFiatCurrenciesResponse response = await _client.SendRequest<SupportedFiatCurrenciesResponse>(url);
                return response.tokens;
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching Sardine supported fiat currencies: " + e.Message);
            }
        }

        public async Task<SardineSupportedToken[]> SardineGetSupportedTokens(bool fullList = false)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetSupportedTokens";
            try {
                SupportedTokenResponse response = await _client.SendRequest<SupportedTokenResponse>(url);
                var tokens =  response.tokens;
                if (fullList)
                {
                    return tokens;
                }
                else
                {
                    return FilterByChain(tokens);
                }
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine supported tokens: " + e.Message);
            }
        }

        public async Task<SardineEnabledToken[]> SardineGetEnabledTokens(bool fullList = false)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetEnabledTokens";
            try {
                EnabledTokenResponse response = await _client.SendRequest<EnabledTokenResponse>(url);
                var tokens =  response.tokens;
                if (fullList)
                {
                    return tokens;
                }
                else
                {
                    return FilterByChain(tokens);
                }
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine enabled tokens: " + e.Message);
            }
        }

        private T[] FilterByChain<T>(T[] tokens) where T : SardineToken
        {
            List<T> filteredTokens = new List<T>();
            foreach (T token in tokens)
            {
                if (token.MatchesChain(_chain))
                {
                    filteredTokens.Add(token);
                }
            }
            return filteredTokens.ToArray();
        }

        private string CheckoutUrl(string clientToken)
        {
#if SEQUENCE_DEV_STACK || SEQUENCE_DEV
            return _sardineSandboxCheckoutUrl + clientToken + _sardineCheckoutUrlSuffix;
#else
            return _sardineCheckoutUrl + clientToken + _sardineCheckoutUrlSuffix;
#endif
        }

        public string CheckoutUrl(SardineNFTCheckout token)
        {
            return CheckoutUrl(token.token);
        }
        
        public void Checkout(SardineNFTCheckout token)
        {
            string url = CheckoutUrl(token);
            Application.OpenURL(url);
        }
    }
}