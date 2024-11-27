using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class Cart
    {
        public CollectibleOrder[] Listings;
        public Dictionary<string, Sprite> CollectibleImagesByOrderId;
        public Dictionary<string, uint> AmountsRequestedByOrderId;

        private ISwap _swap;
        private IReader _reader;
        private Currency[] _currencies;
        
        public Cart(CollectibleOrder[] listings, Dictionary<string, Sprite> collectibleImagesByOrderId, Dictionary<string, uint> amountsRequestedByOrderId, ISwap swap = null, IReader reader = null)
        {
            Listings = listings;
            CollectibleImagesByOrderId = collectibleImagesByOrderId;
            AmountsRequestedByOrderId = amountsRequestedByOrderId;
            
            if (listings == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be created with a non-null and non-empty {typeof(CollectibleOrder[])}");
            }
            
            int listingsLength = listings.Length;
            if (listingsLength == 0)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be created with a non-empty {typeof(CollectibleOrder[])}");
            }
            
            for (int i = 0; i < listingsLength; i++)
            {
                CollectibleOrder listing = listings[i];
                if (!collectibleImagesByOrderId.ContainsKey(listing.order.orderId))
                {
                    throw new ArgumentException(
                        $"Invalid use. {GetType().Name} must be created with a {typeof(Dictionary<string, Sprite>)} that contains all orderIds as keys in the {typeof(CollectibleOrder[])}");
                }
            }
            
            for (int i = 0; i < listingsLength; i++)
            {
                CollectibleOrder listing = listings[i];
                if (!amountsRequestedByOrderId.ContainsKey(listing.order.orderId))
                {
                    throw new ArgumentException(
                        $"Invalid use. {GetType().Name} must be created with a {typeof(Dictionary<string, uint>)} that contains all orderIds as keys in the {typeof(CollectibleOrder[])}");
                }
            }
            
            SetSwap(swap);
            SetReader(reader);
        }
        
        private void SetSwap(ISwap swap)
        {
            _swap = swap;
            if (_swap == null)
            {
                _swap = new CurrencySwap(ChainDictionaries.ChainById[Listings[0].order.chainId.ToString()]);
            }
        }

        private void SetReader(IReader reader)
        {
            _reader = reader;
            if (_reader == null)
            {
                _reader = new MarketplaceReader(ChainDictionaries.ChainById[Listings[0].order.chainId.ToString()]);
            }
            
            FetchCurrencies().ConfigureAwait(false);
        }
        
        private async Task FetchCurrencies()
        {
            _currencies = await _reader.ListCurrencies();
        }

        public Cart(CollectibleOrder listing, Sprite collectibleIcon, uint amount, ISwap swap = null, IReader reader = null)
        {
            Listings = new CollectibleOrder[] {listing};
            CollectibleImagesByOrderId = new Dictionary<string, Sprite> {{listing.order.orderId, collectibleIcon}};
            AmountsRequestedByOrderId = new Dictionary<string, uint> {{listing.order.orderId, amount}};
            
            SetSwap(swap);
            SetReader(reader);
        }
        
        public CollectibleOrder GetOrderByOrderId(string orderId)
        {
            foreach (CollectibleOrder listing in Listings)
            {
                if (listing.order.orderId == orderId)
                {
                    return listing;
                }
            }

            return null;
        }

        public void AddCollectibleToCart(CollectibleOrder order, Sprite collectibleImage, uint amountRequested)
        {
            if (GetOrderByOrderId(order.order.orderId) == null)
            {
                Listings = Listings.AppendObject(order);
            }
            CollectibleImagesByOrderId[order.order.orderId] = collectibleImage;
            AmountsRequestedByOrderId[order.order.orderId] = amountRequested;
        }

        public string GetApproximateTotalInUSD()
        {
            int listings = Listings.Length;
            double total = 0;
            for (int i = 0; i < listings; i++)
            {
                Order order = Listings[i].order;
                uint amountRequested = AmountsRequestedByOrderId[order.orderId];
                total += order.priceUSD * amountRequested;
            }
            
            return total.ToString("F2");
        }

        public async Task<string> GetApproximateTotalInCurrency(Address currencyAddress)
        {
            int listings = Listings.Length;
            double total = 0;
            for (int i = 0; i < listings; i++)
            {
                Order order = Listings[i].order;
                uint amountRequested = AmountsRequestedByOrderId[order.orderId];
                if (order.priceCurrencyAddress == currencyAddress)
                {
                    total += DecimalNormalizer.ReturnToNormal(BigInteger.Parse(order.priceAmount), (int)order.priceDecimals) * amountRequested;
                }
                else
                {
                    try
                    {
                        SwapPrice price = await _swap.GetSwapPrice(currencyAddress, new Address(order.priceCurrencyAddress), order.priceAmount);
                        total += DecimalNormalizer.ReturnToNormal(BigInteger.Parse(price.maxPrice), (int)order.priceDecimals) * amountRequested;
                    }
                    catch (Exception e)
                    {
                        string error =
                            $"Error fetching swap price for buying {order.priceAmount} of {order.priceCurrencyAddress} with {currencyAddress}: {e.Message}";
                        Debug.LogError(error);
                        return error;
                    }
                }
            }
            
            return total.ToString("0.####");
        }

        public Currency GetBestCurrency()
        {
            int listings = Listings.Length;
            Dictionary<string, int> currencyCounts = new Dictionary<string, int>();
            for (int i = 0; i < listings; i++)
            {
                if (Listings[i].order.priceCurrencyAddress == Currency.NativeCurrencyAddress)
                {
                    return _currencies.GetCurrencyByContractAddress(Currency.NativeCurrencyAddress);
                }
                string currencyAddress = Listings[i].order.priceCurrencyAddress;
                if (currencyCounts.ContainsKey(currencyAddress))
                {
                    currencyCounts[currencyAddress]++;
                }
                else
                {
                    currencyCounts[currencyAddress] = 1;
                }
            }
            
            string bestCurrencyAddress = currencyCounts.OrderByDescending(pair => pair.Value).First().Key;
            return _currencies.GetCurrencyByContractAddress(bestCurrencyAddress);
        }
    }
}