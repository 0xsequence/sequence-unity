using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Demo.Mocks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace.Mocks;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sequence.Marketplace
{
    public class CartTests
    {
        private Chain _chain = Chain.ArbitrumNova;
        
        [TestCase(new uint[]{1}, new float[] { 5f }, "5.00")]
        [TestCase(new uint[]{1, 5, 3}, new float[] { 5f, .25f, .25f }, "7.00")]
        [TestCase(new uint[]{100}, new float[] { 1.12345f }, "112.35")]
        [TestCase(new uint[]{1, 2, 5, 3, 2, 10, 27}, new float[] { 5f, 2f, 1.1f, 3.33f, 22f, 1f, .246f }, "85.13")]
        [TestCase(new uint[]{0}, new float[] { 100f }, "0.00")]
        public void TestGetApproximateTotalInUSD(uint[] amounts, float[] pricesUSD, string expected)
        {
            int orderCount = amounts.Length;
            Assert.AreEqual(orderCount, pricesUSD.Length, "Invalid test. Must have same amount of amounts and prices");
            CollectibleOrder[] orders = new CollectibleOrder[orderCount];
            for (int i = 0; i < orderCount; i++)
            {
                orders[i] = CreateFakeOrder(pricesUSD[i]);
            }

            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            for (int i = 0; i < orderCount; i++)
            {
                CollectibleOrder order = orders[i];
                sprites[order.order.orderId] = null;
                quantities[order.order.orderId] = amounts[i];
            }
            Cart cart = new Cart(null, orders, sprites, quantities);
            string totalInUSD = cart.GetApproximateTotalInUSD();
            
            Assert.AreEqual(expected, totalInUSD);
        }
        
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
        public void TestCartCreationError_EmptyListings()
        {
            CollectibleOrder[] orders = new CollectibleOrder[0];
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Cart(null, orders, sprites, quantities));
            Assert.True(exception.Message.Contains("Invalid use."));
        }

        [Test]
        public void TestCartCreationError_NullListings()
        {
            CollectibleOrder[] orders = null;
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Cart(null, orders, sprites, quantities));
            Assert.True(exception.Message.Contains("Invalid use."));
        }
        
        [Test]
        public void TestCartCreationError_MissingSprite()
        {
            CollectibleOrder[] orders = new CollectibleOrder[1];
            orders[0] = CreateFakeOrder(5f);
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Cart(null, orders, sprites, quantities));
            Assert.True(exception.Message.Contains("Invalid use."));
        }
        
        [Test]
        public void TestCartCreationError_MissingQuantity()
        {
            CollectibleOrder[] orders = new CollectibleOrder[1];
            orders[0] = CreateFakeOrder(5f);
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            sprites[orders[0].order.orderId] = null;
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Cart(null, orders, sprites, quantities));
            Assert.True(exception.Message.Contains("Invalid use."));
        }

        [Test]
        public void TestWeCanAddOrderToCart()
        {
            CollectibleOrder[] orders = new CollectibleOrder[]
            {
                CreateFakeOrder(5f),
                CreateFakeOrder(2.25f),
                CreateFakeOrder(1.25f),
                CreateFakeOrder(.99f),
                CreateFakeOrder(.001f),
            };
            
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            for (int i = 0; i < orders.Length; i++)
            {
                sprites[orders[i].order.orderId] = null;
                quantities[orders[i].order.orderId] = (uint)i;
            }
            
            Cart cart = new Cart(null, orders, sprites, quantities);
            
            string totalInUSD = cart.GetApproximateTotalInUSD();
            Assert.AreEqual("7.72", totalInUSD);
            
            CollectibleOrder newOrder = CreateFakeOrder(2f);
            cart.AddCollectibleToCart(newOrder, null, 5);
            
            string newTotalInUSD = cart.GetApproximateTotalInUSD();
            Assert.Greater(float.Parse(newTotalInUSD), float.Parse(totalInUSD));
            Assert.AreEqual("17.72", newTotalInUSD);
        }

        [TestCase(0, 
            new uint[]{1}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{0},
            new ulong[]{1000}, 
            "1")]
        [TestCase(0, 
            new uint[]{1, 3}, 
            new ulong[]{1000, 9000500}, 
            new ulong[]{3, 5}, 
            new uint[]{0, 0},
            new ulong[]{1000, 1000}, 
            "271.015")]
        [TestCase(0, 
            new uint[]{0}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{0},
            new ulong[]{1000}, 
            "0")]
        [TestCase(0, 
            new uint[]{0}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{1},
            new ulong[]{1000}, 
            "0")]
        [TestCase(0, 
            new uint[]{1}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{1},
            new ulong[]{10000}, 
            "10")]
        [TestCase(0, 
            new uint[]{1, 3}, 
            new ulong[]{1000, 9000500}, 
            new ulong[]{3, 5}, 
            new uint[]{0, 1},
            new ulong[]{10000, 10000}, 
            "1.3")]
        public async Task TestGetApproximateTotalInCurrency(int currencyIndex, uint[] amounts, ulong[] priceAmount,
            ulong[] priceDecimals, uint[] priceCurrencyIndex, ulong[] maxSwapPrices, string expected)
        {
            int amountsCount = amounts.Length;
            int priceAmountCount = priceAmount.Length;
            int priceDecimalsCount = priceDecimals.Length;
            int priceCurrencyIndexCount = priceCurrencyIndex.Length;
            int maxSwapPricesCount = maxSwapPrices.Length;
            Assert.AreEqual(amountsCount, priceAmountCount, "Invalid test. Must have same amount of amounts and priceAmount");
            Assert.AreEqual(amountsCount, priceDecimalsCount, "Invalid test. Must have same amount of amounts and priceDecimals");
            Assert.AreEqual(amountsCount, priceCurrencyIndexCount, "Invalid test. Must have same amount of amounts and priceCurrencyIndex");
            Assert.AreEqual(amountsCount, maxSwapPricesCount, "Invalid test. Must have same amount of amounts and maxSwapPrices");
            CollectibleOrder[] orders = new CollectibleOrder[amountsCount];
            for (int i = 0; i < amountsCount; i++)
            {
                orders[i] = CreateFakeOrder(priceCurrencyIndex[i], priceAmount[i], priceDecimals[i]);
            }
            
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            for (int i = 0; i < orders.Length; i++)
            {
                sprites[orders[i].order.orderId] = null;
                quantities[orders[i].order.orderId] = amounts[i];
            }
            
            Cart cart = new Cart(null, orders, sprites, quantities, new MockSwapThatGivesPricesInOrder(maxSwapPrices));
            
            string totalInCurrency = await cart.GetApproximateTotalInCurrency(new Address(_possibleCurrencyAddresses[currencyIndex]));
            Assert.AreEqual(expected, totalInCurrency);
        }

        private CollectibleOrder CreateFakeOrder(uint currencyIndex, ulong amount, BigInteger decimals)
        {
            string[] possibleNames = new []{ "AwesomeToken", "MadeWithSequence", "SequenceSampleToken", "SequenceTestToken", "SequenceToken", "SequenceIsBest" };
            string[] possibleAddresses = new [] { "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0", "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153", "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94", "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C" };
            CollectibleOrder order = new CollectibleOrder(
                new Marketplace.TokenMetadata(Random.Range(1, 10000).ToString(),possibleNames.GetRandomObjectFromArray()),
                new Order(Random.Range(1, 100000), Random.Range(1, 100000), Random.Range(1, 100000), 
                    Random.Range(1, 10000).ToString(), EnumExtensions.GetRandomEnumValue<MarketplaceKind>(), EnumExtensions.GetRandomEnumValue<SourceKind>(), OrderSide.listing,
                    OrderStatus.active, BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]), possibleAddresses.GetRandomObjectFromArray(), 
                    Random.Range(1, 10000).ToString(), possibleAddresses.GetRandomObjectFromArray(), amount.ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), _possibleCurrencyAddresses[currencyIndex],
                    decimals, (decimal)Random.Range(1f, 10000f), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 19), Random.Range(1, 100), null, DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.Add(TimeSpan.FromDays(300)).ToString(CultureInfo.InvariantCulture),
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), ""));
            return order;
        }

        [TestCase(new int[] {1}, 1)]
        [TestCase(new int[] {0,1,2,3}, 0)] // 0 is the default currency for the chain
        [TestCase(new int[] {0,1,2,3,2,2,2,2}, 0)]
        [TestCase(new int[] {3,1,2,1,1,1,1,2,3}, 1)]
        [TestCase(new int[] {3,2,2,2,2,2,2,2,3}, 2)]
        public async Task TestGetBestCurrency(int[] priceCurrencyIndices, int expectedCurrencyIndex)
        {
            int amount = priceCurrencyIndices.Length;
            CollectibleOrder[] orders = new CollectibleOrder[amount];
            for (int i = 0; i < amount; i++)
            {
                orders[i] = CreateFakeOrder((uint)priceCurrencyIndices[i], 1000, 3);
            }
            
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            for (int i = 0; i < orders.Length; i++)
            {
                sprites[orders[i].order.orderId] = null;
                quantities[orders[i].order.orderId] = 1;
            }
            
            Cart cart = new Cart(null, orders, sprites, quantities);

            Currency bestCurrency = await cart.GetBestCurrency();
            Assert.AreEqual(_possibleCurrencyAddresses[expectedCurrencyIndex], bestCurrency.contractAddress);
        }

        [Test]
        public async Task TestGetCurrencyIcon()
        {
            CollectibleOrder listing = CreateFakeOrder(1f);
            Cart cart = new Cart(null, listing, null, 1);
            Currency[] currencies = await cart.GetCurrencies();
            Currency currency = currencies.GetCurrencyByContractAddress(listing.order.priceCurrencyAddress);
            Assert.NotNull(currency);
            
            Sprite sprite = await cart.GetCurrencyIcon(currency);
            
            Assert.NotNull(sprite);
        }

        [Test]
        public void TestGetOrderByOrderId()
        {
            CollectibleOrder listing = CreateFakeOrder(1f);
            Cart cart = new Cart(null, listing, null, 1);
            
            CollectibleOrder order = cart.GetOrderByOrderId(listing.order.orderId);
            
            Assert.NotNull(order);
            Assert.AreEqual(listing, order);
        }

        [TestCase(0, 
            new uint[]{1}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{0},
            new ulong[]{1000}, 
            true,
            "1")]
        [TestCase(0, 
            new uint[]{1, 3}, 
            new ulong[]{1000, 9000500}, 
            new ulong[]{3, 5}, 
            new uint[]{0, 0},
            new ulong[]{1000, 1000}, 
            true,
            "271.015")]
        [TestCase(0, 
            new uint[]{0}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{0},
            new ulong[]{1000}, 
            true,
            "")]
        [TestCase(0, 
            new uint[]{0}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{1},
            new ulong[]{1000}, 
            true,
            "")]
        [TestCase(0, 
            new uint[]{1}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{1},
            new ulong[]{10000}, 
            true,
            "10")]
        [TestCase(3, 
            new uint[]{1, 3}, 
            new ulong[]{1000, 9000500}, 
            new ulong[]{3, 5}, 
            new uint[]{3, 1},
            new ulong[]{10000, 10000}, 
            true,
            "1.3")]
        [TestCase(0, 
            new uint[]{1}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{0},
            new ulong[]{1000}, 
            false,
            "")]
        [TestCase(0, 
            new uint[]{1, 3}, 
            new ulong[]{1000, 9000500}, 
            new ulong[]{3, 5}, 
            new uint[]{0, 0},
            new ulong[]{1000, 1000}, 
            false,
            "")]
        [TestCase(0, 
            new uint[]{0}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{0},
            new ulong[]{1000}, 
            false,
            "")]
        [TestCase(0, 
            new uint[]{1}, 
            new ulong[]{1000}, 
            new ulong[]{3}, 
            new uint[]{1},
            new ulong[]{10000}, 
            false,
            "")]
        [TestCase(3, 
            new uint[]{1, 3}, 
            new ulong[]{1000, 9000500}, 
            new ulong[]{3, 5}, 
            new uint[]{3, 1},
            new ulong[]{10000, 10000}, 
            false,
            "")]
        public async Task TestGetApproximateTotalInCurrencyIfAffordable(int currencyIndex, uint[] amounts, ulong[] priceAmount,
            ulong[] priceDecimals, uint[] priceCurrencyIndex, ulong[] maxSwapPrices, bool canAfford, string expected)
        {
            int amountsCount = amounts.Length;
            int priceAmountCount = priceAmount.Length;
            int priceDecimalsCount = priceDecimals.Length;
            int priceCurrencyIndexCount = priceCurrencyIndex.Length;
            int maxSwapPricesCount = maxSwapPrices.Length;
            Assert.AreEqual(amountsCount, priceAmountCount, "Invalid test. Must have same amount of amounts and priceAmount");
            Assert.AreEqual(amountsCount, priceDecimalsCount, "Invalid test. Must have same amount of amounts and priceDecimals");
            Assert.AreEqual(amountsCount, priceCurrencyIndexCount, "Invalid test. Must have same amount of amounts and priceCurrencyIndex");
            Assert.AreEqual(amountsCount, maxSwapPricesCount, "Invalid test. Must have same amount of amounts and maxSwapPrices");
            CollectibleOrder[] orders = new CollectibleOrder[amountsCount];
            for (int i = 0; i < amountsCount; i++)
            {
                orders[i] = CreateFakeOrder(priceCurrencyIndex[i], priceAmount[i], priceDecimals[i]);
            }
            
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            Dictionary<string, uint> quantities = new Dictionary<string, uint>();
            for (int i = 0; i < orders.Length; i++)
            {
                sprites[orders[i].order.orderId] = null;
                quantities[orders[i].order.orderId] = amounts[i];
            }
            
            Cart cart = new Cart(null, orders, sprites, quantities, new MockSwapThatGivesPricesInOrder(maxSwapPrices));
            
            string totalInCurrency = await cart.GetApproximateTotalInCurrency(new Address(_possibleCurrencyAddresses[currencyIndex]));

            float total = float.Parse(totalInCurrency);

            if (canAfford)
            {
                total += 10000;
            }
            else
            {
                total -= 10000;
                if (total < 0)
                {
                    total = 0;
                }
            }

            int totalRounded = Mathf.RoundToInt(total);
            IIndexer indexer = new MockIndexerReturnsProvidedValue(totalRounded.ToString(), 1);
            
            cart = new Cart(new EOAWalletToSequenceWalletAdapter(new EOAWallet()), orders, sprites, quantities, new MockSwapThatGivesPricesInOrder(maxSwapPrices), null, indexer);
            
            string totalInCurrencyIfAffordable = await cart.GetApproximateTotalInCurrencyIfAffordable(new Address(_possibleCurrencyAddresses[currencyIndex]));
            if (totalInCurrencyIfAffordable.StartsWith("Error"))
            {
                throw new Exception(totalInCurrencyIfAffordable);
            }
            Assert.AreEqual(expected, totalInCurrencyIfAffordable);
        }
    }
}