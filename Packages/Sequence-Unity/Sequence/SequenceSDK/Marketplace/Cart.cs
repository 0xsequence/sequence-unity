using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class Cart
    {
        public IWallet Wallet;
        public CollectibleOrder[] Listings;
        public Dictionary<string, Sprite> CollectibleImagesByOrderId;
        public Dictionary<string, uint> AmountsRequestedByOrderId;

        private ISwap _swap;
        private IReader _reader;
        private IIndexer _indexer;
        private Currency[] _currencies;
        private Dictionary<string, Sprite> _currencyIcons = new Dictionary<string, Sprite>();
        private Chain _chain;

        public Cart(IWallet wallet, CollectibleOrder[] listings, Dictionary<string, Sprite> collectibleImagesByOrderId, Dictionary<string, uint> amountsRequestedByOrderId, ISwap swap = null, IReader reader = null, IIndexer indexer = null)
        {
            Wallet = wallet;
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

            _chain = ChainDictionaries.ChainById[Listings[0].order.chainId.ToString()];
            
            SetSwap(swap);
            SetReader(reader);
            SetIndexer(indexer);
        }
        
        private void SetSwap(ISwap swap)
        {
            _swap = swap;
            if (_swap == null)
            {
                _swap = new CurrencySwap(_chain);
            }
        }

        private void SetReader(IReader reader)
        {
            _reader = reader;
            if (_reader == null)
            {
                _reader = new MarketplaceReader(_chain);
            }
            
            FetchCurrencies().ConfigureAwait(false);
        }

        private void SetIndexer(IIndexer indexer)
        {
            _indexer = indexer;
            if (_indexer == null)
            {
                _indexer = new ChainIndexer(_chain);
            }
        }
        
        private async Task FetchCurrencies()
        {
            _currencies = await _reader.ListCurrencies();
        }

        public Cart(IWallet wallet, CollectibleOrder listing, Sprite collectibleIcon, uint amount, ISwap swap = null, IReader reader = null, IIndexer indexer = null)
        {
            Wallet = wallet;
            Listings = new CollectibleOrder[] {listing};
            CollectibleImagesByOrderId = new Dictionary<string, Sprite> {{listing.order.orderId, collectibleIcon}};
            AmountsRequestedByOrderId = new Dictionary<string, uint> {{listing.order.orderId, amount}};
            
            SetSwap(swap);
            SetReader(reader);
            SetIndexer(indexer);
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
                        return error;
                    }
                }
            }
            
            return total.ToString("0.####");
        }

        public Currency GetBestCurrency()
        {
            Currency defaultChainCurrency = _currencies.FindDefaultChainCurrency();
            string chosenAddress = Currency.NativeCurrencyAddress;
            if (defaultChainCurrency != null)
            {
                chosenAddress = defaultChainCurrency.contractAddress;
            }
            int listings = Listings.Length;
            Dictionary<string, int> currencyCounts = new Dictionary<string, int>();
            for (int i = 0; i < listings; i++)
            {
                if (Listings[i].order.priceCurrencyAddress == chosenAddress)
                {
                    return _currencies.GetCurrencyByContractAddress(chosenAddress);
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

        public async Task<Currency[]> GetCurrencies()
        {
            while (_currencies == null)
            {
                await Task.Yield();
            }
            return _currencies;
        }
        
        public async Task<Sprite> GetCurrencyIcon(Currency currency)
        {
            if (_currencyIcons.TryGetValue(currency.contractAddress, out var icon))
            {
                return icon;
            }
            else
            {
                Sprite currencyIcon = await AssetHandler.GetSpriteAsync(currency.imageUrl);
                _currencyIcons[currency.contractAddress] = currencyIcon;
                return currencyIcon;
            }
        }

        public async Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress)
        {
            string total = await GetApproximateTotalInCurrency(new Address(currencyContractAddress));
            if (string.IsNullOrWhiteSpace(total) || total.StartsWith("Error"))
            {
                return total;
            }
            
            double price = double.Parse(total);
            if (price <= 0)
            {
                return "";
            }

            if (currencyContractAddress != Currency.NativeCurrencyAddress)
            {
                try
                {
                    GetTokenBalancesReturn balancesReturn = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(Wallet.GetWalletAddress(), currencyContractAddress));
                    if (balancesReturn == null || balancesReturn.balances == null)
                    {
                        throw new Exception("Received unexpected null response from indexer");
                    }
                    TokenBalance[] balances = balancesReturn.balances;
                    if (balances.Length == 0)
                    {
                        return "";
                    }
                    TokenBalance balance = balances[0];
                    double balanceAmount = DecimalNormalizer.ReturnToNormal(balance.balance, balance.contractInfo.decimals);
                    if (balanceAmount >= price)
                    {
                        return total;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception e)
                {
                    string error = $"Error fetching token balances for {currencyContractAddress}: {e.Message}";
                    return error;
                }
            }
            else
            {
                try
                {
                    EtherBalance balancesReturn = await _indexer.GetEtherBalance(Wallet.GetWalletAddress());
                    if (balancesReturn == null)
                    {
                        throw new Exception("Received unexpected null response from indexer");
                    }
                    BigInteger balance = balancesReturn.balanceWei;
                    double balanceAmount = DecimalNormalizer.ReturnToNormal(balance);
                    if (balanceAmount >= price)
                    {
                        return total;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception e)
                {
                    string error = $"Error fetching native balance for {_chain}: {e.Message}";
                    return error;
                }
            }
        }
    }
}