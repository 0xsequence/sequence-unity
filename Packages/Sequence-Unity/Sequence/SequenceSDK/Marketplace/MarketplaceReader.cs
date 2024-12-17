using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class MarketplaceReader : IMarketplaceReader
    {
        private Chain _chain;
        private IHttpClient _client;
        
        public MarketplaceReader(Chain chain, IHttpClient client = null)
        {
            _chain = chain;
            if (client == null)
            {
                client = new HttpClient();
            }
            _client = client;
        }

        public event Action<Currency[]> OnListCurrenciesReturn;
        public event Action<string> OnListCurrenciesError;
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
                string errorMessage = $"Error offer currencies: {e.Message}";
                OnListCurrenciesError?.Invoke(errorMessage);
                throw new Exception(errorMessage);
            }
        }
        
        public event Action<ListCollectiblesReturn> OnListCollectibleOrdersReturn;
        public event Action<string> OnListCollectibleOrdersError;
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
                OnListCollectibleOrdersReturn?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                OnListCollectibleOrdersError?.Invoke(e.Message);
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

        public event Action<TokenMetadata> OnGetCollectibleReturn;
        public event Action<string> OnGetCollectibleError;
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

        public event Action<Order> OnGetCollectibleOrderReturn;
        public event Action<string> OnGetCollectibleOrderError;
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
                OnGetCollectibleOrderReturn?.Invoke(order);
                return order;
            }
            catch (Exception e)
            {
                string errorMessage = $"Error getting order for {tokenId} from {contractAddress}: {e.Message}";
                OnGetCollectibleOrderError?.Invoke(errorMessage);
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
        
        public event Action<ListCollectibleListingsReturn> OnListCollectibleListingsReturn;
        public event Action<string> OnListCollectibleListingsError;
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
                string errorMessage = $"Error offer collectible listings for {tokenId} from {contractAddress}: {e.Message}";
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
        
        public event Action<ListCollectibleOffersReturn> OnListCollectibleOffersReturn;
        public event Action<string> OnListCollectibleOffersError;
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
                string errorMessage = $"Error offer collectible offers for {tokenId} from {contractAddress}: {e.Message}";
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

        public async Task<CollectibleOrder> GetFloorOrder(Address contractAddress, CollectiblesFilter filter = null)
        {
            GetFloorOrderArgs args = new GetFloorOrderArgs(contractAddress, filter);
            
            try
            {
                GetOrderResponse response = await _client.SendRequest<GetFloorOrderArgs, GetOrderResponse>(_chain, "GetFloorOrder", args);
                CollectibleOrder order = response.collectible;
                return order;
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting floor order for {contractAddress}: {e.Message}");
            }
        }

        public Task<CollectibleOrder[]> ListAllSellableOffers(Address sellableBy, Address collection,
            CollectiblesFilter filter = default)
        {
            if (filter == null)
            {
                filter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null, new string[] { sellableBy }, null, null, new string[] { sellableBy });
            }
            if (filter.inAccounts == null)
            {
                filter.inAccounts = new string[] { sellableBy };
            }
            else
            {
                filter.inAccounts = ArrayUtils.CombineArrays(filter.inAccounts, new string[] { sellableBy });
            }
            
            if (filter.ordersNotCreatedBy == null)
            {
                filter.ordersNotCreatedBy = new string[] { sellableBy };
            }
            else
            {
                filter.ordersNotCreatedBy = ArrayUtils.CombineArrays(filter.ordersNotCreatedBy, new string[] { sellableBy });
            }
            
            return ListAllCollectibleOffersWithHighestPricedOfferFirst(collection, filter);
        }

        public async Task<CollectibleOrder[]> ListAllPurchasableListings(Address purchasableBy, Address collection,
            IIndexer indexer = null, CollectiblesFilter filter = null)
        {
            if (indexer == null)
            {
                indexer = new ChainIndexer(_chain);
            }

            if (!indexer.ChainMatched(_chain))
            {
                throw new ArgumentException($"Given an indexer configured to fetch from the wrong chain. Given {indexer.GetChain()}, expected {_chain}");
            }

            if (filter == null)
            {
                filter = new CollectiblesFilter(true, "", null, (MarketplaceKind[])null, null, null, null, new string[] { purchasableBy });
            }
            if (filter.ordersNotCreatedBy == null)
            {
                filter.ordersNotCreatedBy = new string[] { purchasableBy };
            }
            else
            {
                filter.ordersNotCreatedBy = ArrayUtils.CombineArrays(filter.ordersNotCreatedBy, new string[] { purchasableBy });
            }
            
            CollectibleOrder[] orders = await ListAllCollectibleListingsWithLowestPricedListingsFirst(collection, filter);
            
            FilterAffordableOrders filterAffordableOrders = new FilterAffordableOrders(purchasableBy, indexer, orders);

            return await filterAffordableOrders.RemoveListingsThatUserCannotAfford();
        }
    }
}