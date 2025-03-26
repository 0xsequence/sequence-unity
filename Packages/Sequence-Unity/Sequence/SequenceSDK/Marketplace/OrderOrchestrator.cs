using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class OrderOrchestrator
    {
        private static Dictionary<string, OrderOrchestrator> _orchestratorsByOrderId = new Dictionary<string, OrderOrchestrator>();

        public bool Initialized { get; private set; }
        public Dictionary<Order, ulong> AmountsByOrder { get; private set; }
        private CollectibleOrder _listing;
        private Order[] _listings;
        private ulong _amountRequested;
        private IMarketplaceReader _marketplaceReader;
        private Page _page;
        private CollectibleOrder[] _orders;
        
        private async Task FetchOtherListings()
        {
            try
            {
                ListCollectibleListingsReturn result =
                    await _marketplaceReader.ListListingsForCollectible(
                        new Address(_listing.order.collectionContractAddress), _listing.order.tokenId,
                        new OrderFilter(null, null, new[] { _listing.order.priceCurrencyAddress }));
                _page = result.page;
                _listings = result.listings;
            }
            catch (Exception e)
            {
                string error =
                    $"Error fetching other listings for collectible (contract: {_listing.order.collectionContractAddress}, tokenId: {_listing.order.tokenId}): {e.Message}";
                Debug.LogError(error);
                Initialized = true;
                throw new Exception(error);
            }
            _listings = _listings.OrderBy(listing => listing.priceUSD).ToArray(); // Todo confirm if this comes pre-sorted by the API

            ulong remaining = await SetAmountsByOrder();
            if (remaining > 0)
            {
                ulong requested = _amountRequested + remaining; // We should already revert _amountRequested to the max available in SetAmountsByOrder
                Debug.LogError($"Amount requested exceeds what is available in the marketplace for collectible (contract: {_listing.order.collectionContractAddress}, tokenId: {_listing.order.tokenId}), amount requested: {requested} available: {_amountRequested}. Setting requested amount to the available amount: {_amountRequested}");
            }
            
            Initialized = true;
        }

        internal Task<ulong> SetAmount(ulong newAmount)
        {
            _amountRequested = newAmount;
            return SetAmountsByOrder();
        }
            
        private async Task<ulong> SetAmountsByOrder()
        {
            AmountsByOrder = new Dictionary<Order, ulong>();
            ulong remaining = _amountRequested;
            foreach (Order order in _listings)
            {
                ulong amount = Math.Min(remaining, ulong.Parse(order.quantityRemaining));
                AmountsByOrder[order] = amount;
                remaining -= amount;
                if (remaining == 0)
                {
                    break;
                }
            }

            if (remaining > 0 && _page.more)
            {
                await FetchMoreListings();
                await SetAmountsByOrder();
            }
            
            _amountRequested -= remaining;
            
            return remaining;
        }

        private async Task FetchMoreListings()
        {
            if (!_page.more)
            {
                return;
            }

            ListCollectibleListingsReturn result =
                await _marketplaceReader.ListListingsForCollectible(
                    new Address(_listing.order.collectionContractAddress), _listing.order.tokenId,
                    new OrderFilter(null, null, new[] { _listing.order.priceCurrencyAddress }), _page);
            _page = result.page;
            _listings.AppendArray(result.listings);
        }

        private OrderOrchestrator()
        {
            throw new NotSupportedException($"Please use {nameof(GetOrchestrator)} to create an instance of {nameof(OrderOrchestrator)}");
        }

        public static OrderOrchestrator GetOrchestrator(CollectibleOrder listing, ulong initialAmount, IMarketplaceReader marketplaceReader = null)
        {
            if (_orchestratorsByOrderId.TryGetValue(listing.order.orderId, out OrderOrchestrator orchestrator))
            {
                return orchestrator;
            }
            else
            {
                _orchestratorsByOrderId[listing.order.orderId] = new OrderOrchestrator(listing, initialAmount, marketplaceReader);
                return _orchestratorsByOrderId[listing.order.orderId];
            }
        }
        
        private OrderOrchestrator(CollectibleOrder listing, ulong initialAmount, IMarketplaceReader marketplaceReader = null)
        {
            _listing = listing;
            _amountRequested = initialAmount;
            _marketplaceReader = marketplaceReader;
            if (_marketplaceReader == null)
            {
                _marketplaceReader = new MarketplaceReader(ChainDictionaries.ChainById[listing.order.chainId.ToString()]);
            }
            FetchOtherListings().ConfigureAwait(false);
        }

        public CollectibleOrder[] GetOrders()
        {
            int expectedLength = AmountsByOrder.Keys.Count;
            if (_orders != null && _orders.Length == expectedLength)
            {
                return _orders;
            }
            
            CollectibleOrder[] orders = new CollectibleOrder[expectedLength];
            int i = 0;
            foreach (var order in AmountsByOrder.Keys)
            {
                orders[i] = new CollectibleOrder(_listing.metadata, order);
                i++;
            }

            _orders = orders;

            return orders;
        }
    }
}