using System;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class MarketplaceReader
    {
        private Chain _chain;
        private IHttpClient _client;
        
        public MarketplaceReader(Chain chain)
        {
            _chain = chain;
            _client = new HttpClient();
        }

        public Action<Currency[]> OnListCurrenciesReturn;
        public Action<string> OnListCurrenciesError;
        public async Task<Currency[]> ListCurrencies()
        {
            try
            {
                ListCurrenciesResponse currenciesResponse = await _client.SendRequest<ListCurrenciesResponse>(_chain, "ListCurrencies");
                Currency[] currencies = currenciesResponse.currencies;
                OnListCurrenciesReturn?.Invoke(currencies);
                return currencies;
            }
            catch (Exception e)
            {
                string errorMessage = $"Error listing currencies: {e.Message}";
                OnListCurrenciesError?.Invoke(errorMessage);
                throw new Exception(errorMessage);
            }
        }
        
        public Action<ListCollectiblesReturn> OnListCollectiblesReturn;
        public Action<string> OnListCollectiblesError;
        public Task<ListCollectiblesReturn> ListCollectiblesWithLowestListing(string contractAddress, CollectiblesFilter filter = default, Page page = default)
        {
            string endpoint = "ListCollectiblesWithLowestListing";

            return DoListCollectibles(endpoint, contractAddress, filter, page);
        }

        private async Task<ListCollectiblesReturn> DoListCollectibles(string endpoint, string contractAddress,
            CollectiblesFilter filter, Page page)
        {
            ListCollectiblesArgs args =
                new ListCollectiblesArgs(contractAddress, filter, page);

            try
            {
                ListCollectiblesReturn result =
                    await _client
                        .SendRequest<ListCollectiblesArgs, ListCollectiblesReturn>(
                            _chain, endpoint, args);
                OnListCollectiblesReturn?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                OnListCollectiblesError?.Invoke(e.Message);
                return null;
            }
        }

        public Task<CollectibleOrder[]> ListAllCollectiblesWithLowestListing(string contractAddress,
            CollectiblesFilter filter = default)
        {
            return ListAllCollectibles("ListCollectiblesWithLowestListing", contractAddress, filter);
        }
        
        private async Task<CollectibleOrder[]> ListAllCollectibles(string endpoint, string contractAddress, CollectiblesFilter filter = default)
        {
            ListCollectiblesReturn result = await DoListCollectibles(endpoint, contractAddress, filter, null);
            if (result == null)
            {
                return null;
            }

            CollectibleOrder[] collectibles = result.collectibles;
            while (result.page != null && result.page.more)
            {
                collectibles = ArrayUtils.CombineArrays(collectibles, result.collectibles);
                result = await DoListCollectibles(endpoint, contractAddress, filter, result.page);
                if (result == null)
                {
                    return collectibles;
                }
            }

            return collectibles;
        }
        
        public Task<ListCollectiblesReturn> ListCollectiblesWithHighestOffer(string contractAddress, CollectiblesFilter filter = default, Page page = default)
        {
            string endpoint = "ListCollectiblesWithHighestOffer";
            return DoListCollectibles(endpoint, contractAddress, filter, page);
        }

        public Task<CollectibleOrder[]> ListAllCollectibleWithHighestOffer(string contractAddress,
            CollectiblesFilter filter = default)
        {
            return ListAllCollectibles("ListCollectiblesWithHighestOffer", contractAddress, filter);
        }

        public Action<TokenMetadata> OnGetCollectibleReturn;
        public Action<string> OnGetCollectibleError;
        public async Task<TokenMetadata> GetCollectible(Address contractAddress, string tokenId)
        {
            GetCollectibleRequest request = new GetCollectibleRequest(contractAddress, tokenId);
            
            try
            {
                TokenMetadata tokenMetadata = await _client.SendRequest<GetCollectibleRequest, TokenMetadata>(_chain, "GetCollectible", request);
                OnGetCollectibleReturn?.Invoke(tokenMetadata);
                return tokenMetadata;
            }
            catch (Exception e)
            {
                string errorMessage = $"Error getting collectible {tokenId} from {contractAddress}: {e.Message}";
                OnGetCollectibleError?.Invoke(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        public Action<Order> GetCollectibleOrderReturn;
        public Action<string> GetCollectibleOrderError;
        public Task<Order> GetCollectibleLowestOffer(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetCollectibleLowestOffer";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }

        private async Task<Order> GetCollectibleOrder(string endpoint, Address contractAddress, string tokenId,
            OrderFilter filter)
        {
            GetCollectibleOrderRequest args = new GetCollectibleOrderRequest(contractAddress, tokenId, filter);
            try
            {
                Order order = await _client.SendRequest<GetCollectibleOrderRequest, Order>(_chain, endpoint, args);
                GetCollectibleOrderReturn?.Invoke(order);
                return order;
            }
            catch (Exception e)
            {
                string errorMessage = $"Error getting order for {tokenId} from {contractAddress}: {e.Message}";
                GetCollectibleOrderError?.Invoke(errorMessage);
                throw new Exception(errorMessage);
            }
        }
        
        public Task<Order> GetCollectibleHighestOffer(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetCollectibleHighestOffer";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }
        
        public Task<Order> GetCollectibleLowestListing(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetCollectibleLowestListing";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }
        
        public Task<Order> GetCollectibleHighestListing(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetCollectibleHighestListing";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }
        
        public Action<ListCollectibleListingsReturn> OnListCollectibleListingsReturn;
        public Action<string> OnListCollectibleListingsError;
        public async Task<ListCollectibleListingsReturn> ListCollectibleListings(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            ListCollectibleListingsArgs args = new ListCollectibleListingsArgs(contractAddress, tokenId, filter, page);
            try
            {
                ListCollectibleListingsReturn result = await _client.SendRequest<ListCollectibleListingsArgs, ListCollectibleListingsReturn>(_chain, "ListCollectibleListings", args);
                OnListCollectibleListingsReturn?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                string errorMessage = $"Error listing collectible listings for {tokenId} from {contractAddress}: {e.Message}";
                OnListCollectibleListingsError?.Invoke(errorMessage);
                throw new Exception(errorMessage);
            }
        }
        
        public async Task<Order[]> ListAllCollectibleListings(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            ListCollectibleListingsReturn result = await ListCollectibleListings(contractAddress, tokenId, filter, null);
            if (result == null)
            {
                return null;
            }

            Order[] orders = result.listings;
            while (result.page != null && result.page.more)
            {
                result = await ListCollectibleListings(contractAddress, tokenId, filter, result.page);
                if (result == null)
                {
                    return orders;
                }
                orders = ArrayUtils.CombineArrays(orders, result.listings);
            }

            return orders;
        }
        
        public Action<ListCollectibleOffersReturn> OnListCollectibleOffersReturn;
        public Action<string> OnListCollectibleOffersError;
        public async Task<ListCollectibleOffersReturn> ListCollectibleOffers(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            ListCollectibleListingsArgs args = new ListCollectibleListingsArgs(contractAddress, tokenId, filter, page);
            try
            {
                ListCollectibleOffersReturn result = await _client.SendRequest<ListCollectibleListingsArgs, ListCollectibleOffersReturn>(_chain, "ListCollectibleOffers", args);
                OnListCollectibleOffersReturn?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                string errorMessage = $"Error listing collectible offers for {tokenId} from {contractAddress}: {e.Message}";
                OnListCollectibleOffersError?.Invoke(errorMessage);
                throw new Exception(errorMessage);
            }
        }
        
        public async Task<Order[]> ListAllCollectibleOffers(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            ListCollectibleOffersReturn result = await ListCollectibleOffers(contractAddress, tokenId, filter, null);
            if (result == null)
            {
                return null;
            }

            Order[] orders = result.offers;
            while (result.page != null && result.page.more)
            {
                result = await ListCollectibleOffers(contractAddress, tokenId, filter, result.page);
                if (result == null)
                {
                    return orders;
                }
                orders = ArrayUtils.CombineArrays(orders, result.offers);
            }

            return orders;
        }
    }
}