using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public class CurrencySwapTests
    {
        private const Chain _chain = Chain.Polygon;
        private const string USDC = "0x2791Bca1f2de4661ED88A30C99A7a9449Aa84174";
        private const string WPOL = "0x0000000000000000000000000000000000001010";
        
        [Test]
        public async Task GetSwapPriceTest()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000000000000000000";
            
            SwapPrice swapPrice = await currencySwap.GetSwapPrice(new Address(USDC), new Address(WPOL), amount);
            
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
            string amount = "1000000000000000000";
            
            SwapPrice[] swapPrices = await currencySwap.GetSwapPrices(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), new Address(USDC), amount);
            
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
            string amount = "1000000000000000000";
            ChainIndexer indexer = new ChainIndexer(_chain);
            Address userWallet = new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07");
            
            SwapQuote swapQuote = await currencySwap.GetSwapQuote(userWallet, new Address(USDC), new Address(WPOL), amount, true);
            GetTokenBalancesReturn balancesReturn =
                await indexer.GetTokenBalances(new GetTokenBalancesArgs(userWallet, WPOL));
            TokenBalance[] balances = balancesReturn.balances;
            Assert.Greater(balances.Length, 0);
            TokenBalance wethBalance = null;
            foreach (var balance in balances)
            {
                if (balance.contractAddress == WPOL)
                {
                    wethBalance = balance;
                    break;
                }
            }
            Assert.IsNotNull(wethBalance);
            BigInteger wethBalanceAmount = wethBalance.balance;
            
            Assert.IsNotNull(swapQuote);
            Assert.AreEqual(WPOL, swapQuote.currencyAddress);
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