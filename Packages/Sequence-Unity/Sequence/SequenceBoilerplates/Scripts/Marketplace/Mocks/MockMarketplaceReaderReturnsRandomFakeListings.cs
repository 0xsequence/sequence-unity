using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEditor;
using Random = UnityEngine.Random;

namespace Sequence.Boilerplates
{
    public class MockMarketplaceReaderReturnsRandomFakeListings : IMarketplaceReader
    {
        private Chain _chain;
        
        public MockMarketplaceReaderReturnsRandomFakeListings(Chain chain)
        {
            _chain = chain;
        }
        
        public event Action<Marketplace.Currency[]> OnListCurrenciesReturn;
        public event Action<string> OnListCurrenciesError;
        public Task<Marketplace.Currency[]> ListCurrencies()
        {
            MarketplaceReader marketplaceReader = new MarketplaceReader(_chain);
            return marketplaceReader.ListCurrencies();
        }

        public event Action<ListCollectiblesReturn> OnListCollectibleOrdersReturn;
        public event Action<string> OnListCollectibleOrdersError;

        public async Task<ListCollectiblesReturn> ListCollectibleListingsWithLowestPricedListingsFirst(string contractAddress, CollectiblesFilter filter = default,
            Page page = default)
        {
            int numListingsToReturn = UnityEngine.Random.Range(30, 50);
            CollectibleOrder[] orders = new CollectibleOrder[numListingsToReturn];
            for (int i = 0; i < numListingsToReturn; i++)
            {
                orders[i] = GenerateRandomListing();
            }

            return new ListCollectiblesReturn(orders, new Page()
            {
                more = Random.Range(0, 2) == 1
            });
        }

        private CollectibleOrder GenerateRandomListing()
        {
            string[] possibleNames = new []{ "AwesomeToken", "MadeWithSequence", "SequenceSampleToken", "SequenceTestToken", "SequenceToken", "SequenceIsBest" };
            string[] possibleAddresses = new [] { "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0x1099542D7dFaF6757527146C0aB9E70A967f71C0", "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa", "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153", "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94", "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C" };
            string[] possibleCurrencyAddresses = new[]
            {
                "0x750ba8b76187092b0d1e87e28daaf484d1b5273b", "0x722e8bdd2ce80a4422e880164f2079488e115365", // The Currencies on Arb Nova as of Nov 21, 2024
                "0x9d0d8dcba30c8b7241da84f922942c100eb1bddc", Marketplace.Currency.NativeCurrencyAddress
            };
            CollectibleOrder order = new CollectibleOrder(
                new Marketplace.TokenMetadata(Random.Range(1, 10000).ToString(),possibleNames.GetRandomObjectFromArray()),
                new Order(Random.Range(1, 100000), Random.Range(1, 100000), Random.Range(1, 100000), 
                    Random.Range(1, 10000).ToString(), EnumExtensions.GetRandomEnumValue<MarketplaceKind>(), EnumExtensions.GetRandomEnumValue<SourceKind>(), OrderSide.listing,
                    OrderStatus.active, BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]), possibleAddresses.GetRandomObjectFromArray(), 
                    Random.Range(1, 10000).ToString(), possibleAddresses.GetRandomObjectFromArray(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), possibleCurrencyAddresses.GetRandomObjectFromArray(),
                    Random.Range(1, 10000), (decimal)Random.Range(1f, 10000f), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(), Random.Range(1, 10000).ToString(),
                    Random.Range(1, 19), Random.Range(1, 100), null, DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.Add(TimeSpan.FromDays(300)).ToString(CultureInfo.InvariantCulture),
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture), ""));
            return order;
        }

        public Task<CollectibleOrder[]> ListAllCollectibleListingsWithLowestPricedListingsFirst(string contractAddress,
            CollectiblesFilter filter = default)
        {
            throw new NotImplementedException();
        }

        public Task<ListCollectiblesReturn> ListCollectibleOffersWithHighestPricedOfferFirst(string contractAddress, CollectiblesFilter filter = default,
            Page page = default)
        {
            throw new NotImplementedException();
        }

        public Task<CollectibleOrder[]> ListAllCollectibleOffersWithHighestPricedOfferFirst(string contractAddress, CollectiblesFilter filter = default)
        {
            throw new NotImplementedException();
        }

        public event Action<Marketplace.TokenMetadata> OnGetCollectibleReturn;
        public event Action<string> OnGetCollectibleError;
        public Task<Marketplace.TokenMetadata> GetCollectible(Address contractAddress, string tokenId)
        {
            throw new NotImplementedException();
        }

        public event Action<Order> OnGetCollectibleOrderReturn;
        public event Action<string> OnGetCollectibleOrderError;
        public Task<Order> GetLowestPriceOfferForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetHighestPriceOfferForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetLowestPriceListingForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetHighestPriceListingForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public event Action<ListCollectibleListingsReturn> OnListCollectibleListingsReturn;
        public event Action<string> OnListCollectibleListingsError;
        public Task<ListCollectibleListingsReturn> ListListingsForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order[]> ListAllListingsForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public event Action<ListCollectibleOffersReturn> OnListCollectibleOffersReturn;
        public event Action<string> OnListCollectibleOffersError;
        public Task<ListCollectibleOffersReturn> ListOffersForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order[]> ListAllOffersForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<CollectibleOrder> GetFloorOrder(Address contractAddress, CollectiblesFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<CollectibleOrder[]> ListAllSellableOffers(Address sellableBy, Address collection, CollectiblesFilter filter = default)
        {
            throw new NotImplementedException();
        }

        public Task<CollectibleOrder[]> ListAllPurchasableListings(Address purchasableBy, Address collection, IIndexer indexer = null,
            CollectiblesFilter filter = null)
        {
            throw new NotImplementedException();
        }
    }
}