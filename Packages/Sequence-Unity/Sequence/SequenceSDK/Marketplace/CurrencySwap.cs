using System;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class CurrencySwap
    {
        private Chain _chain;
        private const uint DefaultSlippagePercentageInBasisPoints = 50;
        private IHttpClient _client;
        private const string BaseUrl = "https://api.sequence.app/rpc/API";
        
        public CurrencySwap(Chain chain, IHttpClient client = null)
        {
            _chain = chain;
            if (client == null)
            {
                client = new HttpClient();
            }
            _client = client;
        }

        public async Task<SwapPrice> GetSwapPrice(Address buyCurrency, Address sellCurrency, string buyAmount, 
            uint slippagePercentInBasisPoints = DefaultSlippagePercentageInBasisPoints)
        {
            GetSwapPriceRequest args = new GetSwapPriceRequest(buyCurrency, sellCurrency, buyAmount, _chain,
                slippagePercentInBasisPoints);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPrice";
            try
            {
                GetSwapPriceResponse response =
                    await _client.SendRequest<GetSwapPriceRequest, GetSwapPriceResponse>(url, args);
                return response.swapPrice;
            }catch (Exception e)
            {
                throw new Exception($"Error fetching swap price for {buyCurrency} and {sellCurrency} with {nameof(buyAmount)} {buyAmount}: {e.Message}");
            }
        }
        
        public async Task<SwapPrice[]> GetSwapPrices(Address userWallet, Address buyCurrency, string buyAmount,
            uint slippagePercentageInBasisPoints = DefaultSlippagePercentageInBasisPoints)
        {
            GetSwapPricesRequest args = new GetSwapPricesRequest(userWallet, buyCurrency, buyAmount, _chain,
                slippagePercentageInBasisPoints);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapPrices";
            try
            {
                GetSwapPricesResponse response =
                    await _client.SendRequest<GetSwapPricesRequest, GetSwapPricesResponse>(url, args);
                return response.swapPrices;
            }catch (Exception e)
            {
                throw new Exception($"Error fetching swap prices for {buyCurrency} with {nameof(buyAmount)} {buyAmount}: {e.Message}");
            }
        }

        public async Task<SwapQuote> GetSwapQuote(Address userWallet, Address buyCurrency, Address sellCurrency,
            string buyAmount, bool includeApprove,
            uint slippagePercentageInBasisPoints = DefaultSlippagePercentageInBasisPoints)
        {
            GetSwapQuoteRequest args = new GetSwapQuoteRequest(userWallet, buyCurrency, sellCurrency, buyAmount, _chain,
                slippagePercentageInBasisPoints, includeApprove);
            string url = BaseUrl.AppendTrailingSlashIfNeeded() + "GetSwapQuote";
            try
            {
                GetSwapQuoteResponse response =
                    await _client.SendRequest<GetSwapQuoteRequest, GetSwapQuoteResponse>(url, args);
                return response.swapQuote;
            }catch (Exception e)
            {
                throw new Exception($"Error fetching swap quote for buying {buyAmount} of {buyCurrency} with {sellCurrency}: {e.Message}");
            }
        }
    }
}