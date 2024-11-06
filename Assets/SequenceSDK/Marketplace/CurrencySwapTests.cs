using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace
{
    public class CurrencySwapTests
    {
        private const Chain _chain = Chain.ArbitrumNova;
        private const string USDC = "0x750ba8b76187092B0D1E87E28daaf484d1b5273b";
        private const string WETH = "0x722E8BdD2ce80A4422E880164f2079488e115365";
        
        [Test]
        public async Task GetSwapPriceTest()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000000000000000000";
            
            SwapPrice swapPrice = await currencySwap.GetSwapPrice(new Address(USDC), new Address(WETH), amount);
            
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
            
            SwapQuote swapQuote = await currencySwap.GetSwapQuote(userWallet, new Address(USDC), new Address(WETH), amount, true);
            GetTokenBalancesReturn balancesReturn =
                await indexer.GetTokenBalances(new GetTokenBalancesArgs(userWallet, WETH));
            TokenBalance[] balances = balancesReturn.balances;
            Assert.Greater(balances.Length, 0);
            TokenBalance wethBalance = null;
            foreach (var balance in balances)
            {
                if (balance.contractAddress == WETH)
                {
                    wethBalance = balance;
                    break;
                }
            }
            Assert.IsNotNull(wethBalance);
            BigInteger wethBalanceAmount = wethBalance.balance;
            
            Assert.IsNotNull(swapQuote);
            Assert.AreEqual(WETH, swapQuote.currencyAddress);
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