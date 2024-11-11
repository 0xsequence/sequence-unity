using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class CurrencySwap
    {
        private Chain _chain;
        private const uint DefaultSlippagePercentage = 5;
        private IHttpClient _client;
        private const string BaseUrl = "https://dev-api.sequence.app/rpc/API";
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

        public async Task<SwapPrice> GetSwapPrice(Address buyCurrency, Address sellCurrency, string buyAmount, 
            uint slippagePercent = DefaultSlippagePercentage)
        {
            GetSwapPriceRequest args = new GetSwapPriceRequest(buyCurrency, sellCurrency, buyAmount, _chain,
                slippagePercent);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPrice";
            try
            {
                GetSwapPriceResponse response =
                    await _client.SendRequest<GetSwapPriceRequest, GetSwapPriceResponse>(url, args);
                return response.swapPrice;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching swap price for {buyCurrency} and {sellCurrency} with {nameof(buyAmount)} {buyAmount}: {e.Message}");
            }
        }
        
        public async Task<SwapPrice[]> GetSwapPrices(Address userWallet, Address buyCurrency, string buyAmount,
            uint slippagePercentage = DefaultSlippagePercentage)
        {
            GetSwapPricesRequest args = new GetSwapPricesRequest(userWallet, buyCurrency, buyAmount, _chain,
                slippagePercentage);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPrices";
            try
            {
                GetSwapPricesResponse response =
                    await _client.SendRequest<GetSwapPricesRequest, GetSwapPricesResponse>(url, args);
                return response.swapPrices;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching swap prices for {buyCurrency} with {nameof(buyAmount)} {buyAmount}: {e.Message}");
            }
        }

        public async Task<SwapQuote> GetSwapQuote(Address userWallet, Address buyCurrency, Address sellCurrency,
            string buyAmount, bool includeApprove,
            uint slippagePercentage = DefaultSlippagePercentage)
        {
            try
            {
                await AssertWeHaveSufficientBalance(userWallet, buyCurrency, sellCurrency, buyAmount,
                    slippagePercentage);
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}");
            }
            
            GetSwapQuoteRequest args = new GetSwapQuoteRequest(userWallet, buyCurrency, sellCurrency, buyAmount, _chain,
                slippagePercentage, includeApprove);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapQuote";
            try
            {
                GetSwapQuoteResponse response =
                    await _client.SendRequest<GetSwapQuoteRequest, GetSwapQuoteResponse>(url, args);
                return response.swapQuote;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}");
            }
        }

        private async Task AssertWeHaveSufficientBalance(Address userWallet, Address buyCurrency, Address sellCurrency,
            string buyAmount, uint slippagePercentage = DefaultSlippagePercentage)
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