using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Marketplace.Mocks;

namespace Sequence.Marketplace
{
    public class MarketplaceReaderUnitTests
    {
        private Chain _chain = Chain.None;
        private Address _testAddress = new Address("0x1234567890123456789012345678901234567890");
        private Address _collection = new Address("0x0987654321098765432109876543210987654321");
        
        [Test]
        public async Task TestListAllSellableOffers_ConstructsAppropriateFilter_noFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                new string[] { _testAddress }, null, null, new string[] { _testAddress });
            await TestListAllSellableOffers_ConstructsAppropriateFilter(expectedFilter, null);
        }

        private async Task TestListAllSellableOffers_ConstructsAppropriateFilter(CollectiblesFilter expectedFilter, CollectiblesFilter filterGiven)
        {
            MarketplaceReader reader = new MarketplaceReader(_chain, new MockClientAssertsExpectedFilter(expectedFilter));

            try
            {
                await reader.ListAllSellableOffers(_testAddress, _collection, filterGiven);
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        }
        
        [Test]
        public async Task TestListAllSellableOffers_ConstructsAppropriateFilter_noAccountsInFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                new string[] { _testAddress }, null, null, new string[] { _testAddress });
            CollectiblesFilter filterGiven = new CollectiblesFilter(true);
            await TestListAllSellableOffers_ConstructsAppropriateFilter(expectedFilter, filterGiven);
        }
        
        [Test]
        public async Task TestListAllSellableOffers_ConstructsAppropriateFilter_existingAccountsInFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                new string[] { _collection, _testAddress }, null, null, new string[] { _collection, _testAddress });
            CollectiblesFilter filterGiven = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                new string[] { _collection }, null, null, new string[] { _collection });
            await TestListAllSellableOffers_ConstructsAppropriateFilter(expectedFilter, filterGiven);
        }
        
        [Test]
        public async Task TestListAllSellableOffers_ConstructsAppropriateFilter_otherStuffInFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "banana", null, new MarketplaceKind[] { MarketplaceKind.aqua_xyz },
                new string[] { _testAddress }, null, null, new string[] { _testAddress });
            CollectiblesFilter filterGiven = new CollectiblesFilter(true, "banana", null, new MarketplaceKind[] { MarketplaceKind.aqua_xyz });
            await TestListAllSellableOffers_ConstructsAppropriateFilter(expectedFilter, filterGiven);
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_ConstructsAppropriateFilter_noFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                null, null, null, new string[] { _testAddress });
            await TestListAllPurchasableListings_ConstructsAppropriateFilter(expectedFilter, null);
        }

        private async Task TestListAllPurchasableListings_ConstructsAppropriateFilter(CollectiblesFilter expectedFilter, CollectiblesFilter filterGiven)
        {
            MarketplaceReader reader = new MarketplaceReader(_chain, new MockClientAssertsExpectedFilter(expectedFilter));

            try
            {
                await reader.ListAllPurchasableListings(_testAddress, _collection, new MockIndexerReturnsNull(), filterGiven);
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_ConstructsAppropriateFilter_noAccountsInFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                null, null, null, new string[] { _testAddress });
            CollectiblesFilter filterGiven = new CollectiblesFilter(true);
            await TestListAllPurchasableListings_ConstructsAppropriateFilter(expectedFilter, filterGiven);
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_ConstructsAppropriateFilter_existingAccountsInFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                new string[] { _collection }, null, null, new string[] { _collection, _testAddress });
            CollectiblesFilter filterGiven = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null,
                new string[] { _collection }, null, null, new string[] { _collection });
            await TestListAllPurchasableListings_ConstructsAppropriateFilter(expectedFilter, filterGiven);
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_ConstructsAppropriateFilter_otherStuffInFilter()
        {
            CollectiblesFilter expectedFilter = new CollectiblesFilter(true, "banana", null, new MarketplaceKind[] { MarketplaceKind.aqua_xyz },
                null, null, null, new string[] { _testAddress });
            CollectiblesFilter filterGiven = new CollectiblesFilter(true, "banana", null, new MarketplaceKind[] { MarketplaceKind.aqua_xyz });
            await TestListAllPurchasableListings_ConstructsAppropriateFilter(expectedFilter, filterGiven);
        }

        [Test]
        public async Task TestListAllPurchasableListings_CorrectListingsAreRemoved()
        {
            string currency1 = "0xc683a014955b75F5ECF991d4502427c8fa1Aa249";
            string currency2 = "0x1099542D7dFaF6757527146C0aB9E70A967f71C0";
            string currency3 = "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa";
            string currency4 = "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153";
            string currency5 = "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94";
            CollectibleOrder[] initialList = new CollectibleOrder[]
            {
                new CollectibleOrder(null, CreateOrder(currency1)),
                new CollectibleOrder(null, CreateOrder(currency2)),
                new CollectibleOrder(null, CreateOrder(currency3)),
                new CollectibleOrder(null, CreateOrder(currency4)),
                new CollectibleOrder(null, CreateOrder(currency5)),
            };
            CollectibleOrder[] expectedList = new CollectibleOrder[]
            {
                new CollectibleOrder(null, CreateOrder(currency1)),
                new CollectibleOrder(null, CreateOrder(currency2)),
            };

            Dictionary<string, GetTokenBalancesReturn> cachedBalances = new Dictionary<string, GetTokenBalancesReturn>()
            {
                { currency1, CreateTokenBalance(BigInteger.Parse("101")) }, // More than enough
                { currency2, CreateTokenBalance(BigInteger.Parse("100")) }, // Exactly enough
                { currency3, CreateTokenBalance(BigInteger.Parse("99")) }, // Not enough
                { currency4, CreateTokenBalance(BigInteger.Parse("0")) }, // Not enough
                { currency5, null }, // No response
            };
            
            MockIndexerReturnsCached mockIndexer = new MockIndexerReturnsCached(cachedBalances);
            MockClientReturnsGiven mockClient = new MockClientReturnsGiven(new ListCollectiblesReturn(initialList));
            
            MarketplaceReader reader = new MarketplaceReader(_chain, mockClient);
            
            CollectibleOrder[] result = await reader.ListAllPurchasableListings(_testAddress, _collection, mockIndexer, null);
            
            CollectionAssert.AreEqual(expectedList, result);
        }
        
        private Order CreateOrder(string currency)
        {
            return new Order(BigInteger.Zero, BigInteger.Zero, BigInteger.Zero,
                "1", MarketplaceKind.sequence_marketplace_v2, SourceKind.sequence_marketplace_v2, OrderSide.listing,
                OrderStatus.active, BigInteger.Zero, _collection, "", "", "100", 
                "100", "100", "100", currency, BigInteger.Zero, 0, 
                "", "", "", "", "", "",
                BigInteger.Zero, BigInteger.Zero, null, "", "", "",
                "", "", "", "");
        }

        private GetTokenBalancesReturn CreateTokenBalance(BigInteger balance)
        {
            return new GetTokenBalancesReturn()
            {
                balances = new TokenBalance[]
                {
                    new TokenBalance()
                    {
                        balance = balance,
                    }
                }
            };
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_NoListings()
        {
            MockClientReturnsGiven mockClient = new MockClientReturnsGiven(new ListCollectiblesReturn(new CollectibleOrder[0]));
            MarketplaceReader reader = new MarketplaceReader(_chain, mockClient);
            
            CollectibleOrder[] result = await reader.ListAllPurchasableListings(_testAddress, _collection, new MockIndexerReturnsNull(), null);
            
            CollectionAssert.IsEmpty(result);
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_NullListings()
        {
            MockClientReturnsGiven mockClient = new MockClientReturnsGiven(new ListCollectiblesReturn(null));
            MarketplaceReader reader = new MarketplaceReader(_chain, mockClient);
            
            CollectibleOrder[] result = await reader.ListAllPurchasableListings(_testAddress, _collection, new MockIndexerReturnsNull(), null);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task TestListAllPurchasableListings_InvalidIndexer()
        {
            MarketplaceReader reader = new MarketplaceReader(_chain);

            try
            {
                CollectibleOrder[] result = await reader.ListAllPurchasableListings(_testAddress, _collection, new MockIndexerWrongChain());
                Assert.Fail("Expected exception");
            }
            catch (Exception e)
            {
                Assert.True(e.Message.Contains("Given an indexer configured to fetch from the wrong chain"));
            }
        }
    }
}