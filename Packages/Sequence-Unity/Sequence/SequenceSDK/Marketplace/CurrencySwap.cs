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
        private IIndexer _indexer;
        
        public CurrencySwap(Chain chain, IHttpClient client = null)
        {
            _chain = chain;
            if (client == null)
            {
                client = new HttpClient();
            }
            _client = client;
            _indexer = new ChainIndexer(_chain);
        }

        public event Action<SwapPrice> OnSwapPriceReturn;
        public event Action<string> OnSwapPriceError;
        
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
            GetSwapPricesRequest args = new GetSwapPricesRequest(userWallet, buyCurrency, buyAmount, _chain,
                slippagePercentage);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPermit2Prices";
            try
            {
                GetSwapPricesResponse response =
                    await _client.SendRequest<GetSwapPricesRequest, GetSwapPricesResponse>(url, args);
                OnSwapPricesReturn?.Invoke(response.swapPermit2Prices);
                return response.swapPermit2Prices;
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
                await AssertWeHaveSufficientBalance(userWallet, buyCurrency, sellCurrency, buyAmount,
                    slippagePercentage);
            }
            catch (Exception e)
            {
                string error = $"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}";
                OnSwapQuoteError?.Invoke(error);
                throw new Exception(error);
            }
            
            GetSwapQuoteRequest args = new GetSwapQuoteRequest(userWallet, buyCurrency, sellCurrency, buyAmount, _chain,
                slippagePercentage, includeApprove);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapQuoteV2";
            try
            {
                GetSwapQuoteResponse response =
                    await _client.SendRequest<GetSwapQuoteRequest, GetSwapQuoteResponse>(url, args);
                if (response.swapQuote == null)
                {
                    string error = $"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: Unknown error - swap API has returned a null response";
                    OnSwapQuoteError?.Invoke(error);
                    throw new Exception(error);
                }
                OnSwapQuoteReturn?.Invoke(response.swapQuote);
                return response.swapQuote;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}";
                OnSwapQuoteError?.Invoke(error);
                throw new Exception(error);
            }
        }

        private async Task AssertWeHaveSufficientBalance(Address userWallet, Address buyCurrency, Address sellCurrency,
            string buyAmount, uint slippagePercentage = ISwap.DefaultSlippagePercentage)
        {
            BigInteger required, have;
            try
            {
                SwapPrice price = await GetSwapPrice(buyCurrency, sellCurrency, buyAmount, slippagePercentage);
                required = BigInteger.Parse(price.maxPrice);
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching swap price for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}");
            }

            TokenBalance[] sellCurrencyBalances;
            try
            {
                GetTokenBalancesReturn balanceResponse = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(userWallet, sellCurrency));
                sellCurrencyBalances = balanceResponse.balances;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching token balance of {sellCurrency}: {e.Message}");
            }

            if (sellCurrencyBalances == null || sellCurrencyBalances.Length == 0)
            {
                have = 0;
            }
            else
            {
                have = sellCurrencyBalances[0].balance;
            }

            if (have < required)
            {
                throw new Exception(
                    $"Insufficient balance of {sellCurrency} to buy {buyAmount} of {buyCurrency}, have {have}, need {required}");
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
                    Debug.LogError($"Received multiple {nameof(route.toTokens)} in response from Lifi: {responseJson}");
                }
            }
        }

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