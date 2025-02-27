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

        [Test]
        public async Task TestGetCollectiblePrices()
        {
            Address boredApes = new Address("0xbc4ca0eda7647a8ab7c2061c2e118a18a936f13d");
            PriceFeed priceFeed = new PriceFeed();

            TokenPrice[] prices = await priceFeed.GetCollectiblePrices(
                new PriceFeed.Token(Chain.Ethereum, boredApes, "7623"),
                new PriceFeed.Token(Chain.Ethereum, boredApes, "7471"),
                new PriceFeed.Token(Chain.Ethereum, boredApes, "7720"));

            Assert.NotNull(prices);
            Assert.AreEqual(3, prices.Length);
            Assert.AreEqual(boredApes, prices[0].token.Contract);
            Assert.AreEqual(boredApes, prices[1].token.Contract);
            Assert.AreEqual(boredApes, prices[2].token.Contract);
            Assert.Greater(prices[0].price.value, 0);
            Assert.Greater(prices[1].price.value, 0);
            Assert.Greater(prices[2].price.value, 0);
        }
    }
}