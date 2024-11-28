using System;
using System.Threading.Tasks;
using Sequence;
using Sequence.Marketplace;
using Random = UnityEngine.Random;

namespace Temp
{
    public class MockSwapGivesRandomExchangeRate : ISwap
    {
        public event Action<SwapPrice> OnSwapPriceReturn;
        public event Action<string> OnSwapPriceError;

        public async Task<SwapPrice> GetSwapPrice(Address buyCurrency, Address sellCurrency, string buyAmount,
            uint slippagePercent = ISwap.DefaultSlippagePercentage)
        {
            return new SwapPrice(sellCurrency, "", "", Random.Range(1, 100).ToString(), "");
        }

        public event Action<SwapPrice[]> OnSwapPricesReturn;
        public event Action<string> OnSwapPricesError;

        public Task<SwapPrice[]> GetSwapPrices(Address userWallet, Address buyCurrency, string buyAmount,
            uint slippagePercentage = ISwap.DefaultSlippagePercentage)
        {
            throw new NotImplementedException();
        }

        public event Action<SwapQuote> OnSwapQuoteReturn;
        public event Action<string> OnSwapQuoteError;

        public Task<SwapQuote> GetSwapQuote(Address userWallet, Address buyCurrency, Address sellCurrency, string buyAmount, bool includeApprove,
            uint slippagePercentage = ISwap.DefaultSlippagePercentage)
        {
            throw new NotImplementedException();
        }
    }
}