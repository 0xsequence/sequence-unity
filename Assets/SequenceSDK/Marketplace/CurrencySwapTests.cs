using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public class CurrencySwapTests
    {
        private const Chain _chain = Chain.ArbitrumOne;
        private const string USDC = "0xaf88d065e77c8cC2239327C5EDb3A432268e5831";
        private const string USDCe = "0xff970a61a04b1ca14834a43f5de4533ebddb5cc8";
        
        [Test]
        public async Task GetSwapPriceTest()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000";
            
            SwapPrice swapPrice = await currencySwap.GetSwapPrice(new Address(USDC), new Address(USDCe), amount);
            
            Assert.IsNotNull(swapPrice);
            Assert.AreEqual(USDC, swapPrice.currencyAddress);
            Assert.AreEqual(amount, swapPrice.currencyBalance);
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.price));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.maxPrice));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.transactionValue));
            Assert.GreaterOrEqual(BigInteger.Parse(swapPrice.transactionValue), 0);
        }

        [Test]
        public async Task GetSwapQuotesTest()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000";
            
            SwapPrice[] swapPrices = await currencySwap.GetSwapPrices(new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108"), new Address(USDC), amount);
            
            Assert.IsNotNull(swapPrices);
            Assert.Greater(swapPrices.Length, 0);
            foreach (SwapPrice swapPrice in swapPrices)
            {
                Assert.IsNotNull(swapPrice);
                Assert.AreEqual(USDC, swapPrice.currencyAddress);
                Assert.AreEqual(amount, swapPrice.currencyBalance);
                Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.price));
                Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.maxPrice));
                Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.transactionValue));
                Assert.GreaterOrEqual(BigInteger.Parse(swapPrice.transactionValue), 0);
            }
        }
        
        [Test]
        public async Task GetSwapQuoteTest()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000";
            ChainIndexer indexer = new ChainIndexer(_chain);
            Address userWallet = new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108");
            
            SwapQuote swapQuote = await currencySwap.GetSwapQuote(userWallet, new Address(USDC), new Address(USDCe), amount, true);
            GetTokenBalancesReturn balancesReturn =
                await indexer.GetTokenBalances(new GetTokenBalancesArgs(userWallet, USDCe));
            TokenBalance[] balances = balancesReturn.balances;
            Assert.Greater(balances.Length, 0);
            TokenBalance wethBalance = null;
            foreach (var balance in balances)
            {
                if (balance.contractAddress == USDCe)
                {
                    wethBalance = balance;
                    break;
                }
            }
            Assert.IsNotNull(wethBalance);
            BigInteger wethBalanceAmount = wethBalance.balance;
            
            Assert.IsNotNull(swapQuote);
            Assert.AreEqual(USDCe, swapQuote.currencyAddress);
            Assert.AreEqual(wethBalanceAmount, BigInteger.Parse(swapQuote.currencyBalance));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.price));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.maxPrice));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.transactionData));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.transactionValue));
            Assert.GreaterOrEqual(BigInteger.Parse(swapQuote.transactionValue), 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.approveData));
        }
    }
}