using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Pay.Sardine;
using Sequence.Marketplace;
using UnityEngine;

namespace Sequence.Pay.Tests.Sardine
{
    public class SardineCheckoutTests
    {
        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);

        [Test]
        public async Task TestCheckSardineWhitelistStatus()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            bool result =
                await sardine.CheckSardineWhitelistStatus(new Address("0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb"));

            Assert.False(result);
            
            
            ERC1155Sale saleContract = new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b");
            result =
                await sardine.CheckSardineWhitelistStatus(saleContract);

            Assert.True(result);
        }

        [Test]
        public async Task TestGetSardineSupportedRegions()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);
            
            SardineRegion[] regions = await sardine.SardineGetSupportedRegions();
            
            Assert.NotNull(regions);
            Assert.Greater(regions.Length, 0);
            foreach (SardineRegion region in regions)
            {
                Assert.NotNull(region);
                Assert.IsFalse(string.IsNullOrWhiteSpace(region.name));
            }
        }

        [Test]
        public async Task TestSardineGetQuote()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);
            
            SardineEnabledToken[] tokens = await sardine.SardineGetEnabledTokens();
            Assert.NotNull(tokens);
            Assert.Greater(tokens.Length, 0);
            SardineEnabledToken token = tokens[0];

            SardineQuote quote = await sardine.SardineGetQuote(token, 30);
            
            Assert.NotNull(quote);
            Assert.AreEqual("USD", quote.currency);
        }

        [Test]
        public async Task TestSardineGetClientToken()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            string token = await sardine.SardineGetClientToken();
            
            Assert.NotNull(token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            Debug.Log(token);
        }

        [Test]
        public async Task TestSardineGetNFTCheckoutToken_SecondarySale_ERC1155()
        {
            CollectibleOrder[] collectibleOrders = await OrderFetcher.FetchListings(Chain.Polygon, "0x0ee3af1874789245467e7482f042ced9c5171073");
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            SardineNFTCheckout token = await sardine.SardineGetNFTCheckoutToken(collectibleOrders, 1);
            
            Assert.NotNull(token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.token));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.expiresAt));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.orderId));
            Debug.Log(sardine.CheckoutUrl(token));
        }

        [Test]
        public async Task TestSardineGetNFTCheckoutToken_PrimarySale_ERC1155()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);
            ERC1155Sale saleContract = new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b");
            ERC1155 collection = new ERC1155("0xdeb398f41ccd290ee5114df7e498cf04fac916cb");

            SardineNFTCheckout token = await sardine.SardineGetNFTCheckoutToken(saleContract, collection.Contract.GetAddress(), 1, 1);
            
            Assert.NotNull(token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.token));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.expiresAt));
            Assert.IsFalse(string.IsNullOrWhiteSpace(token.orderId));
        }

        [Test]
        public async Task TestSardineGetNFTCheckoutToken_PrimarySale_ERC1155_NativeCurrency()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);
            ERC1155Sale saleContract = new ERC1155Sale("0xf0056139095224f4eec53c578ab4de1e227b9597");
            ERC1155 collection = new ERC1155("0x92473261f2c26f2264429c451f70b0192f858795");

            try
            {
                SardineNFTCheckout token =
                    await sardine.SardineGetNFTCheckoutToken(saleContract, collection.Contract.GetAddress(), 1, 1);
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Sardine checkout does not support native currency checkout; please choose a sales contract with a different payment token", e.Message);
            }
        }

        [Test]
        public async Task TestSardineGetSupportedFiatCurrencies()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);
            
            SardineFiatCurrency[] currencies = await sardine.SardineGetSupportedFiatCurrencies();
            
            Assert.NotNull(currencies);
            Assert.Greater(currencies.Length, 0);
            foreach (SardineFiatCurrency currency in currencies)
            {
                Assert.NotNull(currency);
                Assert.IsFalse(string.IsNullOrWhiteSpace(currency.currencyCode));
                Assert.IsFalse(string.IsNullOrWhiteSpace(currency.name));
            }
        }

        [Test]
        public async Task TestSardineGetSupportedTokens()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            SardineSupportedToken[] tokens = await sardine.SardineGetSupportedTokens(true);
            
            Assert.NotNull(tokens);
            Assert.Greater(tokens.Length, 0);
            foreach (var token in tokens)
            {
                Assert.NotNull(token);
                Assert.IsFalse(string.IsNullOrWhiteSpace(token.network));
            }

            int fullLength = tokens.Length;
            
            tokens = await sardine.SardineGetSupportedTokens();
            
            Assert.NotNull(tokens);
            Assert.Greater(tokens.Length, 0);
            foreach (var token in tokens)
            {
                Assert.NotNull(token);
                Assert.IsFalse(string.IsNullOrWhiteSpace(token.network));
                Assert.IsTrue(token.MatchesChain(Chain.Polygon));
            }

            Assert.Less(tokens.Length, fullLength);
        }

        [Test]
        public async Task TestSardineGetEnabledTokens()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon, _testWallet);

            SardineEnabledToken[] tokens = await sardine.SardineGetEnabledTokens(true);
            
            Assert.NotNull(tokens);
            Assert.Greater(tokens.Length, 0);
            foreach (var token in tokens)
            {
                Assert.NotNull(token);
                Assert.IsFalse(string.IsNullOrWhiteSpace(token.network));
            }

            int fullLength = tokens.Length;
            
            tokens = await sardine.SardineGetEnabledTokens();
            
            Assert.NotNull(tokens);
            Assert.Greater(tokens.Length, 0);
            foreach (var token in tokens)
            {
                Assert.NotNull(token);
                Assert.IsFalse(string.IsNullOrWhiteSpace(token.network));
                Assert.IsTrue(token.MatchesChain(Chain.Polygon));
            }

            Assert.Less(tokens.Length, fullLength);
        }
    }
}