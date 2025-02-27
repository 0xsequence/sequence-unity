using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Indexer.Tests
{
    public class PriceFeedTests
    {
        [Test]
        public async Task TestGetCoinPrices()
        {
            Address polygonUSDC = new Address("0x3c499c542cef5e3811e1192ce70d8cc03d5c3359");
            Address arbitrumNovaUSDC = new Address("0x750ba8b76187092B0D1E87E28daaf484d1b5273b");
            PriceFeed.Token polygonUSDCtoken = new PriceFeed.Token(Chain.Polygon, polygonUSDC);
            PriceFeed.Token arbitrumNovaUSDCtoken = new PriceFeed.Token(Chain.ArbitrumNova, arbitrumNovaUSDC);
            PriceFeed priceFeed = new PriceFeed();

            TokenPrice[] prices = await priceFeed.GetCoinPrices(polygonUSDCtoken, arbitrumNovaUSDCtoken);

            Assert.NotNull(prices);
            Assert.AreEqual(2, prices.Length);
            Assert.AreEqual(polygonUSDC, prices[0].token.Contract);
            Assert.AreEqual(arbitrumNovaUSDC, prices[1].token.Contract);
            Assert.AreEqual(1, Math.Round(prices[0].price.value));
            Assert.AreEqual(1, Math.Round(prices[1].price.value));
        }
    }
}