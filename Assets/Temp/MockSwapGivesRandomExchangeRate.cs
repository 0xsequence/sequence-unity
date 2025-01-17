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

        public async Task<SwapQuote> GetSwapQuote(Address userWallet, Address buyCurrency, Address sellCurrency, string buyAmount, bool includeApprove,
            uint slippagePercentage = ISwap.DefaultSlippagePercentage)
        {
            return new SwapQuote(buyCurrency, buyAmount, "some price", "max price",
                new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), "data", "value", "approveData");
        }
    }
}