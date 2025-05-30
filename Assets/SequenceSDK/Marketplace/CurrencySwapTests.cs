using System;
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
            
            SwapPrice swapPrice = await currencySwap.GetSwapPrice(new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108"), new Address(USDC), new Address(USDCe), amount);
            
            Assert.IsNotNull(swapPrice);
            Assert.AreEqual(USDCe.ToLower(), swapPrice.currencyAddress.Value.ToLower());
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.price));
        }

        [Test]
        public async Task GetSwapPricesTest()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000";
            
            SwapPrice[] swapPrices = await currencySwap.GetSwapPrices(new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108"), new Address(USDC), amount);
            
            Assert.IsNotNull(swapPrices);
            Assert.Greater(swapPrices.Length, 0);
            foreach (SwapPrice swapPrice in swapPrices)
            {
                Assert.IsNotNull(swapPrice);
                Assert.IsFalse(string.IsNullOrWhiteSpace(swapPrice.price));
                Assert.NotNull(swapPrice.currencyAddress);
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
            Assert.AreEqual(USDCe.ToLower(), swapQuote.currencyAddress.Value.ToLower());
            Assert.AreEqual(wethBalanceAmount, BigInteger.Parse(swapQuote.currencyBalance));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.price));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.maxPrice));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.transactionData));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.transactionValue));
            Assert.GreaterOrEqual(BigInteger.Parse(swapQuote.transactionValue), BigInteger.Zero);
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.approveData));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.amount));
            Assert.IsFalse(string.IsNullOrWhiteSpace(swapQuote.amountMin));
        }
        
        [Test]
        public async Task GetSwapQuoteTest_InsufficientBalance()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000";
            ChainIndexer indexer = new ChainIndexer(_chain);
            Address userWallet = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");

            try
            {
                SwapQuote swapQuote = await currencySwap.GetSwapQuote(userWallet, new Address(USDC), new Address(USDCe), amount, true);
                Assert.Fail("Exception expected but none was encountered");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("not enough balance for swap"));
            }
        }
        
        [Test]
        public async Task GetSwapQuoteTest_FailedToFetchPrice()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            string amount = "1000000000000000000000000000000000000";
            ChainIndexer indexer = new ChainIndexer(_chain);
            Address userWallet = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");

            try
            {
                SwapQuote swapQuote = await currencySwap.GetSwapQuote(userWallet, new Address(USDC), new Address(USDCe), amount, true);
                Assert.Fail("Exception expected but none was encountered");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Error fetching swap quote"));
            }
        }

        [Test]
        public async Task TestGetSupportedChains()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);

            try
            {
                Chain[] supportedChains = await currencySwap.GetSupportedChains();
                Assert.IsNotNull(supportedChains);
                Assert.Greater(supportedChains.Length, 0);
            }
            catch (Exception e)
            {
                Assert.Fail($"Exception encountered while fetching supported chains: {e.Message}");
            }
        }

        [Test]
        public async Task TestGetSupportedTokens()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            
            try
            {
                Token[] supportedTokens = await currencySwap.GetSupportedTokens(new[] { _chain });
                Assert.IsNotNull(supportedTokens);
                Assert.Greater(supportedTokens.Length, 0);
            }
            catch (Exception e)
            {
                Assert.Fail($"Exception encountered while fetching supported tokens: {e.Message}");
            }
        }
        
        [Test]
        public async Task TestGetSupportedTokens_EmptyChains()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            
            try
            {
                Token[] supportedTokens = await currencySwap.GetSupportedTokens(new Chain[0]);
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Error fetching supported tokens:"));
            }
        }
        
        [Test]
        public async Task TestGetSupportedTokens_NullChains()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            
            try
            {
                Token[] supportedTokens = await currencySwap.GetSupportedTokens(null);
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Error fetching supported tokens:"));
            }
        }

        [Test]
        public async Task TestGetSupportedTokens_AllSupportedChains()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);

            try
            {
                Chain[] supportedChains = await currencySwap.GetSupportedChains();
                Assert.IsNotNull(supportedChains);
                Assert.Greater(supportedChains.Length, 0);
                
                Token[] supportedTokens = await currencySwap.GetSupportedTokens(supportedChains);
                Assert.IsNotNull(supportedTokens);
                Assert.Greater(supportedTokens.Length, 0);
            }
            catch (Exception e)
            {
                Assert.Fail($"Exception encountered while fetching supported tokens: {e.Message}");
            }
        }

        [Test]
        public async Task TestGetLifiSwapRoutes()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            try
            {
                LifiSwapRoute[] swapRoutes = await currencySwap.GetLifiSwapRoutes(
                    new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108"), new Address(USDC), "1000");
                Assert.NotNull(swapRoutes);
                Assert.Greater(swapRoutes.Length, 0);
            }
            catch (Exception e)
            {
                Assert.Fail($"Exception encountered while fetching Lifi swap routes: {e.Message}");
            }
        }

        [Test]
        public async Task TestGetLifiSwapQuote()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            try
            {
                LifiSwapQuote swapQuote = await currencySwap.GetLifiSwapQuote(
                    new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108"), new Address(USDC), new Address(USDCe),
                    "1000");
                Assert.NotNull(swapQuote);
                Assert.AreEqual(USDCe.ToLower(), swapQuote.currencyAddress.Value.ToLower());
            }
            catch (Exception e)
            {
                Assert.Fail($"Exception encountered while fetching Lifi swap quote: {e.Message}");
            }
        }

        [Test]
        public async Task TestGetLifiSwapQuote_NoAmounts()
        {
            CurrencySwap currencySwap = new CurrencySwap(_chain);
            try
            {
                LifiSwapQuote swapQuote = await currencySwap.GetLifiSwapQuote(
                    new Address("0xe8db071f698aBA1d60babaE8e08F5cBc28782108"), new Address(USDC), new Address(USDCe));
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Error fetching Lifi swap quote:"));
            }
        }
    }
}