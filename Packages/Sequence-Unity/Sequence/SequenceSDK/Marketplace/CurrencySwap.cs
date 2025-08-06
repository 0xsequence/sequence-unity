using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class CurrencySwap : ISwap
    {
        private Chain _chain;
        private IHttpClient _client;
#if SEQUENCE_DEV_STACK || SEQUENCE_DEV
        private const string BaseUrl = "https://dev-api.sequence.app/rpc/API";
#else        
        private const string BaseUrl = "https://api.sequence.app/rpc/API";
#endif
        
        public CurrencySwap(Chain chain, IHttpClient client = null)
        {
            _chain = chain;
            if (client == null)
            {
                client = new HttpClient();
            }
            _client = client;
        }

        public event Action<SwapPrice> OnSwapPriceReturn;
        public event Action<string> OnSwapPriceError;

        public async Task<SwapPrice> GetSwapPrice(Address userWallet, Address buyCurrency, Address sellCurrency, string buyAmount)
        {
            try
            {
                SwapPrice[] swapPrices = await GetSwapPrices(userWallet, buyCurrency, buyAmount);
                if (swapPrices == null || swapPrices.Length == 0)
                {
                    throw new Exception("No swap path with sufficient liquidity found");
                }
                
                foreach (SwapPrice swapPrice in swapPrices)
                {
                    if (swapPrice.currencyAddress.Equals(sellCurrency))
                    {
                        OnSwapPriceReturn?.Invoke(swapPrice);
                        return swapPrice;
                    }
                }
                
                throw new Exception("No swap path with sufficient liquidity found");
            }
            catch (Exception e)
            {
                string error = $"Error fetching swap price for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}";
                OnSwapPriceError?.Invoke(error);
                throw new Exception(error);
            }
        }

        [Obsolete("Swap provider no longer supports fetching swap prices without provider the user's wallet address")]
        public async Task<SwapPrice> GetSwapPrice(Address buyCurrency, Address sellCurrency, string buyAmount, 
            uint slippagePercent = ISwap.DefaultSlippagePercentage)
        {
            GetSwapPriceRequest args = new GetSwapPriceRequest(buyCurrency, sellCurrency, buyAmount, _chain,
                slippagePercent);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPermit2Price";
            try
            {
                GetSwapPriceResponse response =
                    await _client.SendRequest<GetSwapPriceRequest, GetSwapPriceResponse>(url, args);
                if (response.swapPermit2Price == null)
                {
                    throw new Exception("No swap path with sufficient liquidity found");
                }
                OnSwapPriceReturn?.Invoke(response.swapPermit2Price);
                return response.swapPermit2Price;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching swap price for {buyCurrency} and {sellCurrency} with {nameof(buyAmount)} {buyAmount}: {e.Message}";
                OnSwapPriceError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public event Action<SwapPrice[]> OnSwapPricesReturn;
        public event Action<string> OnSwapPricesError;
        
        public async Task<SwapPrice[]> GetSwapPrices(Address userWallet, Address buyCurrency, string buyAmount,
            uint slippagePercentage = ISwap.DefaultSlippagePercentage)
        {
            try
            {
                LifiSwapRoute[] swapRoutes = await GetLifiSwapRoutes(userWallet, buyCurrency, buyAmount);
                List<SwapPrice> swapPrices = new List<SwapPrice>();
                foreach (var route in swapRoutes)
                {
                    if (route.fromChainId.ToString() != ChainDictionaries.ChainIdOf[_chain])
                    {
                        continue;
                    }
                    if (route.toChainId.ToString() != ChainDictionaries.ChainIdOf[_chain])
                    {
                        continue;
                    }

                    if (!route.toTokens.ContainsToken(buyCurrency))
                    {
                        continue;
                    }

                    foreach (var fromToken in route.fromTokens)
                    {
                        swapPrices.Add(new SwapPrice(fromToken.Contract, fromToken.Price.ToString()));
                    }
                }

                if (swapPrices.Count == 0)
                {
                    throw new SystemException($"Unable to find a swap route with the appropriate {nameof(buyCurrency)} address {buyCurrency} in the response from the swap provider");
                }
                
                return swapPrices.ToArray();
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching swap prices for {buyCurrency} with {nameof(buyAmount)} {buyAmount}: {e.Message}";
                OnSwapPricesError?.Invoke(error);
                throw new Exception(error);
            }
        }
        
        public event Action<SwapQuote> OnSwapQuoteReturn;
        public event Action<string> OnSwapQuoteError;

        public async Task<SwapQuote> GetSwapQuote(Address userWallet, Address buyCurrency, Address sellCurrency,
            string buyAmount, bool includeApprove,
            uint slippagePercentage = ISwap.DefaultSlippagePercentage)
        {
            try
            {
                LifiSwapQuote swapQuote = await GetLifiSwapQuote(userWallet, buyCurrency, sellCurrency, buyAmount, null,
                    includeApprove, slippagePercentage);
                SwapQuote quote = new SwapQuote(swapQuote);
                OnSwapQuoteReturn?.Invoke(quote);
                return quote;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}";
                OnSwapQuoteError?.Invoke(error);
                throw new Exception(error);
            }
        }

        public async Task<Chain[]> GetSupportedChains()
        {
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetLifiChains";
            try
            {
                LifiSupportedChainsResponse response =
                    await _client.SendRequest<object, LifiSupportedChainsResponse>(url, null);
                return response.GetChains();
            }
            catch (Exception e)
            {
                string error = $"Error fetching supported chains: {e.Message}";
                throw new Exception(error);
            }
        }

        public async Task<Token[]> GetSupportedTokens(Chain[] chains)
        {
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetLifiTokens";
            try
            {
                GetLifiTokensResponse response =
                    await _client.SendRequest<GetLifiTokensRequest, GetLifiTokensResponse>(url, new GetLifiTokensRequest(chains));
                return response.tokens;
            }
            catch (Exception e)
            {
                string error = $"Error fetching supported tokens: {e.Message}";
                throw new Exception(error);
            }
        }

        /// <summary>
        /// Base integration of GetLifiSwapRoutes API
        /// In general, it is not recommended to use this method directly - use GetSwapPrice or GetSwapPrices instead
        /// </summary>
        /// <param name="userWallet"></param>
        /// <param name="buyCurrency"></param>
        /// <param name="buyAmount"></param>
        /// <returns></returns>
        [Obsolete("Use GetSwapPrices or GetSwapPrice instead")]
        public async Task<LifiSwapRoute[]> GetLifiSwapRoutes(Address userWallet, Address buyCurrency, string buyAmount)
        {
            GetLifiSwapRoutesArgs args =
                new GetLifiSwapRoutesArgs(BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]), buyCurrency, buyAmount,
                    userWallet);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetLifiSwapRoutes";
            try
            {
                LifiSwapRoutesResponse response = await _client.SendRequest<GetLifiSwapRoutesArgs, LifiSwapRoutesResponse>(url, args);
                if (response.routes == null || response.routes.Length == 0)
                {
                    throw new Exception("No swap path with sufficient liquidity found");
                }

                ValidateNoExtraToTokens(response.routes);
                return response.routes;
            }
            catch (Exception e)
            {
                string error = $"Error fetching Lifi swap routes: {e.Message}";
                throw new Exception(error);
            }
        }

        private void ValidateNoExtraToTokens(LifiSwapRoute[] routes)
        {
            foreach (var route in routes)
            {
                if (route.toTokens.Length > 1)
                {
                    LifiSwapRoutesResponse response = new LifiSwapRoutesResponse(routes);
                    string responseJson = JsonConvert.ToString(response);
                    SequenceLog.Info($"Received multiple {nameof(route.toTokens)} in response from Lifi: {responseJson}");
                }
            }
        }

        /// <summary>
        /// Base integration of GetLifiSwapQuote API
        /// In general, it is not recommended to use this method directly - use GetSwapQuote instead
        /// </summary>
        /// <param name="userWallet"></param>
        /// <param name="buyCurrency"></param>
        /// <param name="fromCurrency"></param>
        /// <param name="buyAmount"></param>
        /// <param name="fromAmount"></param>
        /// <param name="includeApprove"></param>
        /// <param name="slippageInBasisPoints"></param>
        /// <returns></returns>
        [Obsolete("Use GetSwapQuote instead")]
        public async Task<LifiSwapQuote> GetLifiSwapQuote(Address userWallet, Address buyCurrency, Address fromCurrency, string buyAmount = null, string fromAmount = null, bool includeApprove = true, ulong slippageInBasisPoints = 50)
        {
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetLifiSwapQuote";
            try
            {
                GetLifiSwapQuoteParams args =
                    new GetLifiSwapQuoteParams(ulong.Parse(ChainDictionaries.ChainIdOf[_chain]), userWallet,
                        fromCurrency, buyCurrency, includeApprove, slippageInBasisPoints, 
                        buyAmount, fromAmount);
                GetLifiSwapQuoteResponse response =
                    await _client.SendRequest<GetLifiSwapQuoteRequest, GetLifiSwapQuoteResponse>(url, new GetLifiSwapQuoteRequest(args));
                if (response.quote == null)
                {
                    throw new Exception("No swap path with sufficient liquidity found");
                }

                return response.quote;
            }
            catch (Exception e)
            {
                string error = $"Error fetching Lifi swap quote: {e.Message}";
                throw new Exception(error);
            }
        }
    }
}