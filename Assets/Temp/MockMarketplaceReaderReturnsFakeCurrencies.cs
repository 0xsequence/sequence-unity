using System;
using System.Threading.Tasks;
using Sequence;
using Sequence.Marketplace;
using TokenMetadata = Sequence.Marketplace.TokenMetadata;

namespace Temp
{
    public class MockMarketplaceReaderReturnsFakeCurrencies : IMarketplaceReader
    {
        public event Action<Currency[]> OnListCurrenciesReturn;
        public event Action<string> OnListCurrenciesError;
        public async Task<Currency[]> ListCurrencies()
        {
            string[] possibleCurrencyAddresses = new[]
            {
                "0x750ba8b76187092b0d1e87e28daaf484d1b5273b", "0x722e8bdd2ce80a4422e880164f2079488e115365", // The Currencies on Arb Nova as of Nov 21, 2024
                "0x9d0d8dcba30c8b7241da84f922942c100eb1bddc", Sequence.Marketplace.Currency.NativeCurrencyAddress
            };
            Currency[] currencies = new Currency[possibleCurrencyAddresses.Length];
            for (int i = 0; i < possibleCurrencyAddresses.Length; i++)
            {
                currencies[i] = new Currency((ulong)i, Chain.None, possibleCurrencyAddresses[i], "FakeCurrency" + i, "FC" + i, (ulong)18, "", (ulong)1, false, "", "");
            }

            return currencies;
        }

        public event Action<ListCollectiblesReturn> OnListCollectibleOrdersReturn;
        public event Action<string> OnListCollectibleOrdersError;

        public Task<ListCollectiblesReturn> ListCollectibleListingsWithLowestPricedListingsFirst(string contractAddress, CollectiblesFilter filter = default,
            Page page = default)
        {
            throw new NotImplementedException();
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

        public event Action<TokenMetadata> OnGetCollectibleReturn;
        public event Action<string> OnGetCollectibleError;
        public Task<TokenMetadata> GetCollectible(Address contractAddress, string tokenId)
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