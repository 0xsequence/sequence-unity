using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class MarketplaceReaderTests
    {
        private static List<Chain> chainIdCases = EnumExtensions.GetEnumValuesAsList<Chain>();
        
        private int successEvents;
        private int failEvents;

        [SetUp]
        public void Setup()
        {
            successEvents = 0;
            failEvents = 0;
        }

        private void OnSuccess<T>(T result)
        {
            successEvents++;
        }

        private void OnError(string error)
        {
            failEvents++;
            Assert.Fail(error);
        }
        
        [Test]
        public async Task TestListAllCollectiblesWithLowestListing()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            marketplaceReader.OnListCollectiblesReturn += OnSuccess;
            marketplaceReader.OnListCollectiblesError += OnError;
            
            CollectibleOrder[] collectibles = await marketplaceReader.ListAllCollectiblesWithLowestListing(contractAddress, filter);
            
            Assert.IsNotNull(collectibles);
            Assert.Greater(successEvents, 0);
            Assert.AreEqual(0, failEvents);
            Assert.Greater(collectibles.Length, 0);
        }
        
        [Test]
        public async Task TestListAllCollectiblesWithHighestOffer()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            marketplaceReader.OnListCollectiblesReturn += OnSuccess;
            marketplaceReader.OnListCollectiblesError += OnError;
            
            CollectibleOrder[] collectibles = await marketplaceReader.ListAllCollectibleWithHighestOffer(contractAddress, filter);
            
            Assert.IsNotNull(collectibles);
            Assert.Greater(successEvents, 0);
            Assert.AreEqual(0, failEvents);
            Assert.Greater(collectibles.Length, 0);
        }

        [TestCaseSource(nameof(chainIdCases))]
        public async Task TestListCurrencies(Chain chain)
        {
            if (chain == Chain.None)
            {
                return;
            }
            
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            
            marketplaceReader.OnListCurrenciesReturn += OnSuccess;
            marketplaceReader.OnListCurrenciesError += OnError;
            
            Currency[] currencies = await marketplaceReader.ListCurrencies();
            
            Assert.IsNotNull(currencies);
            Assert.AreEqual(1, successEvents);
            Assert.AreEqual(0, failEvents);
            Assert.Greater(currencies.Length, 0);
        }

        [Test]
        public async Task TestGetCollectible()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            string tokenId = "130";
            
            marketplaceReader.OnGetCollectibleReturn += OnSuccess;
            marketplaceReader.OnGetCollectibleError += OnError;
            
            TokenMetadata collectible = await marketplaceReader.GetCollectible(new Address(contractAddress), tokenId);
            
            Assert.IsNotNull(collectible);
            Assert.AreEqual(1, successEvents);
            Assert.AreEqual(0, failEvents);
            Assert.AreEqual(tokenId, collectible.tokenId);
        }

        [Test]
        public async Task TestGetCollectibleLowestOffer()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            string tokenId = "130";
            
            marketplaceReader.GetCollectibleOrderReturn += OnSuccess;
            marketplaceReader.GetCollectibleOrderError += OnError;
            
            Order order = await marketplaceReader.GetCollectibleLowestOffer(new Address(contractAddress), tokenId);

            Assert.IsNotNull(order);
            Assert.AreEqual(1, successEvents);
            Assert.AreEqual(0, failEvents);
            Assert.AreEqual(tokenId, order.tokenId);
        }

        [Test]
        public async Task TestGetCollectibleHighestOffer()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            string tokenId = "130";
            
            marketplaceReader.GetCollectibleOrderReturn += OnSuccess;
            marketplaceReader.GetCollectibleOrderError += OnError;
            
            Order order = await marketplaceReader.GetCollectibleHighestOffer(new Address(contractAddress), tokenId);

            Assert.IsNotNull(order);
            Assert.AreEqual(1, successEvents);
            Assert.AreEqual(0, failEvents);
            Assert.AreEqual(tokenId, order.tokenId);
        }

        [Test]
        public async Task TestGetCollectibleLowestListing()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            string tokenId = "130";
            
            marketplaceReader.GetCollectibleOrderReturn += OnSuccess;
            marketplaceReader.GetCollectibleOrderError += OnError;
            
            Order order = await marketplaceReader.GetCollectibleLowestListing(new Address(contractAddress), tokenId);

            Assert.IsNotNull(order);
            Assert.AreEqual(1, successEvents);
            Assert.AreEqual(0, failEvents);
            Assert.AreEqual(tokenId, order.tokenId);
        }

        [Test]
        public async Task TestGetCollectibleHighestListing()
        {
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            string tokenId = "130";
            
            marketplaceReader.GetCollectibleOrderReturn += OnSuccess;
            marketplaceReader.GetCollectibleOrderError += OnError;
            
            Order order = await marketplaceReader.GetCollectibleHighestListing(new Address(contractAddress), tokenId);

            Assert.IsNotNull(order);
            Assert.AreEqual(1, successEvents);
            Assert.AreEqual(0, failEvents);
            Assert.AreEqual(tokenId, order.tokenId);
        }
    }
}