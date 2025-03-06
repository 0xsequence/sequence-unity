using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace.Mocks;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.Marketplace
{
    public class NftCheckoutTests
    {
        private Chain _chain = Chain.ArbitrumNova;
        
        string[] _possibleCurrencyAddresses = new[]
        {
            "0x750ba8b76187092b0d1e87e28daaf484d1b5273b", "0x722e8bdd2ce80a4422e880164f2079488e115365", // The Currencies on Arb Nova as of Jan 16, 2025
            "0xc721b6d2bcc4d04b92df8f383beef85aa72c2198", Currency.NativeCurrencyAddress
        };

        private CollectibleOrder CreateFakeOrder(float priceUSD)
        {
            string[] possibleNames = new []{ "AwesomeToken", "MadeWithSequence", "SequenceSampleToken", "SequenceTestToken", "SequenceToken", "SequenceIsBest" };
            string[] possibleAddresses = new [] { "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0", "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153", "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94", "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C" };
            CollectibleOrder order = new CollectibleOrder(
                new Marketplace.TokenMetadata(Random.Range(1, 10000).ToString(),possibleNames.GetRandomObjectFromArray()),
                new Order(Random.Range(1, 100000), Random.Range(1, 100000), Random.Range(1, 100000), 
                    Random.Range(1, 10000).ToString(), EnumExtensions.GetRandomEnumValue<MarketplaceKind>(), EnumExtensions.GetRandomEnumValue<SourceKind>(), OrderSide.listing,
                    OrderStatus.active, BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]), possibleAddresses.GetRandomObjectFromArray(), 
                    Random.Range(1, 10000).ToString(), possibleAddresses.GetRandomObjectFromArray(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), _possibleCurrencyAddresses.GetRandomObjectFromArray(),
                    Random.Range(1, 10000), (decimal)priceUSD, Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 19), Random.Range(1, 100), null, DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.Add(TimeSpan.FromDays(300)).ToString(CultureInfo.InvariantCulture),
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), ""));
            return order;
        }

        [Test]
        public void TestCartCreationError_NullListing()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new NftCheckout(null, null, null, 5));
            Assert.True(exception.Message.Contains("Invalid use."));
        }

        [Test]
        public async Task TestGetCartItemData()
        {
            CollectibleOrder orders = CreateFakeOrder(5f);
            
            NftCheckout cart = new NftCheckout(null, orders, null, 5);
            
            CartItemData[] data = cart.GetCartItemData();
            
            Assert.NotNull(data);
            Assert.AreEqual(1, data.Length);
            Assert.False(string.IsNullOrWhiteSpace(data[0].Name));
            Assert.False(string.IsNullOrWhiteSpace(data[0].TokenId));
            Assert.AreEqual(_chain, data[0].Network);
        }
    }
}