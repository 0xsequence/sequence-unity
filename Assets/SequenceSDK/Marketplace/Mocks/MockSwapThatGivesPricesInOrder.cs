using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.Marketplace.Mocks
{
    public class MockSwapThatGivesPricesInOrder : ISwap
    {
        private Queue<SwapPrice> _toReturn;

        public MockSwapThatGivesPricesInOrder(ulong[] maxPrices)
        {
            _toReturn = new Queue<SwapPrice>();
            for (int i = 0; i < maxPrices.Length; i++)
            {
                _toReturn.Enqueue(new SwapPrice(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), "0", "0", maxPrices[i].ToString(), "0"));
            }
        }
        
        public event Action<SwapPrice> OnSwapPriceReturn;
        public event Action<string> OnSwapPriceError;

        public async Task<SwapPrice> GetSwapPrice(Address buyCurrency, Address sellCurrency, string buyAmount,
            uint slippagePercent = ISwap.DefaultSlippagePercentage)
        {
            if (_toReturn.TryDequeue(out SwapPrice returnValue))
            {
                return returnValue;
            }

            throw new Exception("Out of stuff to return");
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

        public Task<Chain[]> GetSupportedChains()
        {
            throw new NotImplementedException();
        }

        public Task<Token[]> GetSupportedTokens(Chain[] chains)
        {
            throw new NotImplementedException();
        }

        public Task<LifiSwapRoute[]> GetLifiSwapRoutes(Address userWallet, Address buyCurrency, string buyAmount)
        {
            throw new NotImplementedException();
        }
    }
}