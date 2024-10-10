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
        public Task<ListCollectiblesReturn> ListCollectibleListingsWithLowestPricedListingsFirst(string contractAddress, CollectiblesFilter filter = default, Page page = default)
        {
            return DoListCollectibles(OrderSide.listing, contractAddress, filter, page);
        }

        private async Task<ListCollectiblesReturn> DoListCollectibles(OrderSide side, string contractAddress,
            CollectiblesFilter filter, Page page)
        {
            ListCollectiblesArgs args =
                new ListCollectiblesArgs(side, contractAddress, filter, page);

            try
            {
                ListCollectiblesReturn result =
                    await _client
                        .SendRequest<ListCollectiblesArgs, ListCollectiblesReturn>(
                            _chain, "ListCollectibles", args);
                OnListCollectiblesReturn?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                OnListCollectiblesError?.Invoke(e.Message);
                return null;
            }
        }

        public Task<CollectibleOrder[]> ListAllCollectibleListingsWithLowestPricedListingsFirst(string contractAddress,
            CollectiblesFilter filter = default)
        {
            return ListAllCollectibles(OrderSide.listing, contractAddress, filter);
        }
        
        private async Task<CollectibleOrder[]> ListAllCollectibles(OrderSide side, string contractAddress, CollectiblesFilter filter = default)
        {
            ListCollectiblesReturn result = await DoListCollectibles(side, contractAddress, filter, null);
            if (result == null)
            {
                return null;
            }

            CollectibleOrder[] collectibles = result.collectibles;
            while (result.page != null && result.page.more)
            {
                collectibles = ArrayUtils.CombineArrays(collectibles, result.collectibles);
                result = await DoListCollectibles(side, contractAddress, filter, result.page);
                if (result == null)
                {
                    return collectibles;
                }
            }

            return collectibles;
        }
        
        public Task<ListCollectiblesReturn> ListCollectibleOffersWithHighestPricedOfferFirst(string contractAddress, CollectiblesFilter filter = default, Page page = default)
        {
            string endpoint = "ListCollectibleOffersWithHighestPricedOfferFirst";
            return DoListCollectibles(OrderSide.offer, contractAddress, filter, page);
        }

        public Task<CollectibleOrder[]> ListAllCollectibleOffersWithHighestPricedOfferFirst(string contractAddress,
            CollectiblesFilter filter = default)
        {
            return ListAllCollectibles(OrderSide.offer, contractAddress, filter);
        }

        public Action<TokenMetadata> OnGetCollectibleReturn;
        public Action<string> OnGetCollectibleError;
        public async Task<TokenMetadata> GetCollectible(Address contractAddress, string tokenId)
        {
            GetCollectibleRequest request = new GetCollectibleRequest(contractAddress, tokenId);
            
            try
            {
                GetCollectibleResponse response = await _client.SendRequest<GetCollectibleRequest, GetCollectibleResponse>(_chain, "GetCollectible", request);
                TokenMetadata tokenMetadata = response.metadata;
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
        public Task<Order> GetLowestPriceOfferForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetLowestPriceOfferForCollectible";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }

        private async Task<Order> GetCollectibleOrder(string endpoint, Address contractAddress, string tokenId,
            OrderFilter filter)
        {
            GetCollectibleOrderRequest args = new GetCollectibleOrderRequest(contractAddress, tokenId, filter);
            try
            {
                OrderResponse orderResponse = await _client.SendRequest<GetCollectibleOrderRequest, OrderResponse>(_chain, endpoint, args);
                Order order = orderResponse.order;
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
        
        public Task<Order> GetHighestPriceOfferForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetHighestPriceOfferForCollectible";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }
        
        public Task<Order> GetLowestPriceListingForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetLowestPriceListingForCollectible";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }
        
        public Task<Order> GetHighestPriceListingForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            string endpoint = "GetHighestPriceListingForCollectible";
            return GetCollectibleOrder(endpoint, contractAddress, tokenId, filter);
        }
        
        public Action<ListCollectibleListingsReturn> OnListCollectibleListingsReturn;
        public Action<string> OnListCollectibleListingsError;
        public async Task<ListCollectibleListingsReturn> ListListingsForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            ListCollectibleListingsArgs args = new ListCollectibleListingsArgs(contractAddress, tokenId, filter, page);
            try
            {
                ListCollectibleListingsReturn result = await _client.SendRequest<ListCollectibleListingsArgs, ListCollectibleListingsReturn>(_chain, "ListListingsForCollectible", args);
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
        
        public async Task<Order[]> ListAllListingsForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            ListCollectibleListingsReturn result = await ListListingsForCollectible(contractAddress, tokenId, filter, null);
            if (result == null)
            {
                return null;
            }

            Order[] orders = result.listings;
            while (result.page != null && result.page.more)
            {
                result = await ListListingsForCollectible(contractAddress, tokenId, filter, result.page);
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
        public async Task<ListCollectibleOffersReturn> ListOffersForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            ListCollectibleListingsArgs args = new ListCollectibleListingsArgs(contractAddress, tokenId, filter, page);
            try
            {
                ListCollectibleOffersReturn result = await _client.SendRequest<ListCollectibleListingsArgs, ListCollectibleOffersReturn>(_chain, "ListOffersForCollectible", args);
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
        
        public async Task<Order[]> ListAllOffersForCollectible(Address contractAddress, string tokenId, OrderFilter filter = null)
        {
            ListCollectibleOffersReturn result = await ListOffersForCollectible(contractAddress, tokenId, filter, null);
            if (result == null)
            {
                return null;
            }

            Order[] orders = result.offers;
            while (result.page != null && result.page.more)
            {
                result = await ListOffersForCollectible(contractAddress, tokenId, filter, result.page);
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