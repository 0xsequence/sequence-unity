using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class CurrencySwap : ISwap
    {
        private Chain _chain;
        private IHttpClient _client;
        private const string BaseUrl = "https://api.sequence.app/rpc/API";
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
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPrice";
            try
            {
                GetSwapPriceResponse response =
                    await _client.SendRequest<GetSwapPriceRequest, GetSwapPriceResponse>(url, args);
                OnSwapPriceReturn?.Invoke(response.swapPrice);
                return response.swapPrice;
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
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPrices";
            try
            {
                GetSwapPricesResponse response =
                    await _client.SendRequest<GetSwapPricesRequest, GetSwapPricesResponse>(url, args);
                OnSwapPricesReturn?.Invoke(response.swapPrices);
                return response.swapPrices;
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
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapQuote";
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
    }
}