using System;
using System.Threading.Tasks;

namespace Sequence.Marketplace
{
    public interface ISwap
    {
        public const uint DefaultSlippagePercentage = 5;

        public event Action<SwapPrice> OnSwapPriceReturn;
        public event Action<string> OnSwapPriceError;
        
        /// <summary>
        /// Get the current SwapPrice for a given buyCurrency, sellCurrency, and buyAmount 
        /// </summary>
        /// <param name="buyCurrency"></param>
        /// <param name="sellCurrency"></param>
        /// <param name="buyAmount"></param>
        /// <param name="slippagePercent">the maximum slippage percentage allowed</param>
        /// <returns></returns>
        public Task<SwapPrice> GetSwapPrice(Address buyCurrency, Address sellCurrency, string buyAmount, uint slippagePercent = DefaultSlippagePercentage);

        public event Action<SwapPrice[]> OnSwapPricesReturn;
        public event Action<string> OnSwapPricesError;
        
        /// <summary>
        /// Get a SwapPrice[] for a given buyCurrency and buyAmount using the available sell currencies in the userWallet
        /// </summary>
        /// <param name="userWallet"></param>
        /// <param name="buyCurrency"></param>
        /// <param name="buyAmount"></param>
        /// <param name="slippagePercentage">the maximum slippage percentage allowed</param>
        /// <returns></returns>
        public Task<SwapPrice[]> GetSwapPrices(Address userWallet, Address buyCurrency, string buyAmount, uint slippagePercentage = DefaultSlippagePercentage);
        
        public event Action<SwapQuote> OnSwapQuoteReturn;
        public event Action<string> OnSwapQuoteError;

        /// <summary>
        /// Get a SwapQuote for a given buyCurrency, sellCurrency, and buyAmount, executable by userWallet
        /// </summary>
        /// <param name="userWallet"></param>
        /// <param name="buyCurrency"></param>
        /// <param name="sellCurrency"></param>
        /// <param name="buyAmount"></param>
        /// <param name="includeApprove">if true, the requested swap quote will include transaction data required to approve the needed amount of sellCurrency for spending by the "to" address (the full-filler of the SwapQuote)</param>
        /// <param name="slippagePercentage">the maximum slippage percentage allowed</param>
        /// <returns></returns>
        public Task<SwapQuote> GetSwapQuote(Address userWallet, Address buyCurrency, Address sellCurrency,
            string buyAmount, bool includeApprove, uint slippagePercentage = DefaultSlippagePercentage);

        /// <summary>
        /// Get a Chain[] of all supported chains for the swap provider
        /// </summary>
        /// <returns></returns>
        public Task<Chain[]> GetSupportedChains();

        /// <summary>
        /// Get the supported tokens by the swap provider for a given set of chains
        /// </summary>
        /// <param name="chains"></param>
        /// <returns></returns>
        public Task<Token[]> GetSupportedTokens(Chain[] chains);
    }
}