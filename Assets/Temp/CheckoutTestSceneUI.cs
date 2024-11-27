using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Temp
{
    public class CheckoutTestSceneUI : MonoBehaviour
    {
        private CheckoutPanel _checkoutPanel;
        private Chain _chain = Chain.ArbitrumNova;

        private void Start()
        {
            _checkoutPanel = FindObjectOfType<CheckoutPanel>();

            int listings = Random.Range(10, 31);
            CollectibleOrder[] collectibleOrders = new CollectibleOrder[listings];
            for (int i = 0; i < listings; i++)
            {
                collectibleOrders[i] = CreateMockCollectibleOrder();
            }
            Dictionary<string, Sprite> collectibleImagesByOrderId = new Dictionary<string, Sprite>();
            Dictionary<string, uint> amountsRequestedByOrderId = new Dictionary<string, uint>();
            for (int i = 0; i < listings; i++)
            {
                collectibleImagesByOrderId.Add(collectibleOrders[i].order.orderId, null);
                amountsRequestedByOrderId.Add(collectibleOrders[i].order.orderId, (uint)Random.Range(0, 1000));
            }
            Cart cart = new Cart(collectibleOrders, collectibleImagesByOrderId, amountsRequestedByOrderId, new MockSwapGivesRandomExchangeRate(), new MockMarketplaceReaderReturnsFakeCurrencies());
            
            _checkoutPanel.Open(cart, new EOAWalletToSequenceWalletAdapter(new EOAWallet()));
        }

        private CollectibleOrder CreateMockCollectibleOrder()
        {
            string[] possibleNames = new []{ "AwesomeToken", "MadeWithSequence", "SequenceSampleToken", "SequenceTestToken", "SequenceToken", "SequenceIsBest" };
            string[] possibleAddresses = new [] { "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0", "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153", "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94", "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C" };
            string[] possibleCurrencyAddresses = new[]
            {
                "0x750ba8b76187092b0d1e87e28daaf484d1b5273b", "0x722e8bdd2ce80a4422e880164f2079488e115365", // The Currencies on Arb Nova as of Nov 21, 2024
                "0x9d0d8dcba30c8b7241da84f922942c100eb1bddc", Sequence.Marketplace.Currency.NativeCurrencyAddress
            };
            CollectibleOrder order = new CollectibleOrder(
                new Sequence.Marketplace.TokenMetadata(Random.Range(1, 10000).ToString(),possibleNames.GetRandomObjectFromArray()),
                new Order(Random.Range(1, 100000), Random.Range(1, 100000), Random.Range(1, 100000), 
                    Random.Range(1, 10000).ToString(), EnumExtensions.GetRandomEnumValue<MarketplaceKind>(), EnumExtensions.GetRandomEnumValue<SourceKind>(), OrderSide.listing,
                    OrderStatus.active, BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]), possibleAddresses.GetRandomObjectFromArray(), 
                    Random.Range(1, 10000).ToString(), possibleAddresses.GetRandomObjectFromArray(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), possibleCurrencyAddresses.GetRandomObjectFromArray(),
                    Random.Range(1, 19), Random.Range(1f, 10000f), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 19), Random.Range(1, 100), null, DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.Add(TimeSpan.FromDays(300)).ToString(CultureInfo.InvariantCulture),
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), ""));
            return order;
        }
    }
}