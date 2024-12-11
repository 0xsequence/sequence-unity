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
    public class Cart : ICheckoutHelper // Todo - refactor to re-use some of the code in NftCheckout
    {
        private IWallet _wallet;
        private CollectibleOrder[] _listings;
        private Dictionary<string, Sprite> _collectibleImagesByOrderId;
        private Dictionary<string, uint> _amountsRequestedByOrderId;
        private ISwap _swap;
        private IMarketplaceReader _marketplaceReader;
        private IIndexer _indexer;
        private ICheckout _checkout;
        private Currency[] _currencies;
        private Dictionary<string, Sprite> _currencyIcons = new Dictionary<string, Sprite>();
        private Chain _chain;
        private Currency _chosenCurrency;

        public Cart(IWallet wallet, CollectibleOrder[] listings, Dictionary<string, Sprite> collectibleImagesByOrderId, Dictionary<string, uint> amountsRequestedByOrderId, ISwap swap = null, IMarketplaceReader marketplaceReader = null, IIndexer indexer = null, ICheckout checkout = null)
        {
            _wallet = wallet;
            _listings = listings;
            _collectibleImagesByOrderId = collectibleImagesByOrderId;
            _amountsRequestedByOrderId = amountsRequestedByOrderId;
            
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
            
            Setup(swap, marketplaceReader, indexer, checkout);
        }

        private void Setup(ISwap swap, IMarketplaceReader marketplaceReader, IIndexer indexer, ICheckout checkout)
        {
            _chain = ChainDictionaries.ChainById[_listings[0].order.chainId.ToString()];
            SetSwap(swap);
            SetReader(marketplaceReader);
            SetIndexer(indexer);
            SetCheckout(checkout);

            ICheckoutHelper.OnSelectedCurrency += currency =>
            {
                if (_currencies.GetCurrencyByContractAddress(currency.contractAddress) == null)
                {
                    return;
                }
                _chosenCurrency = currency;
            };
        }
        
        private void SetSwap(ISwap swap)
        {
            _swap = swap;
            if (_swap == null)
            {
                _swap = new CurrencySwap(_chain);
            }
        }

        private void SetReader(IMarketplaceReader marketplaceReader)
        {
            _marketplaceReader = marketplaceReader;
            if (_marketplaceReader == null)
            {
                _marketplaceReader = new MarketplaceReader(_chain);
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

        private void SetCheckout(ICheckout checkout)
        {
            _checkout = checkout;
            if (_checkout == null)
            {
                _checkout = new Checkout(_wallet, _chain);
            }
        }
        
        private async Task FetchCurrencies()
        {
            _currencies = await _marketplaceReader.ListCurrencies();
            _chosenCurrency = _currencies.FindDefaultChainCurrency();
        }

        public Cart(IWallet wallet, CollectibleOrder listing, Sprite collectibleIcon, uint amount, ISwap swap = null, IMarketplaceReader marketplaceReader = null, IIndexer indexer = null, ICheckout checkout = null)
        {
            _wallet = wallet;
            _listings = new CollectibleOrder[] {listing};
            _collectibleImagesByOrderId = new Dictionary<string, Sprite> {{listing.order.orderId, collectibleIcon}};
            _amountsRequestedByOrderId = new Dictionary<string, uint> {{listing.order.orderId, amount}};
            
            Setup(swap, marketplaceReader, indexer, checkout);
        }
        
        public CollectibleOrder GetOrderByOrderId(string orderId)
        {
            foreach (CollectibleOrder listing in _listings)
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
                _listings = _listings.AppendObject(order);
            }
            _collectibleImagesByOrderId[order.order.orderId] = collectibleImage;
            _amountsRequestedByOrderId[order.order.orderId] = amountRequested;
        }

        public string GetApproximateTotalInUSD()
        {
            int listings = _listings.Length;
            double total = 0;
            for (int i = 0; i < listings; i++)
            {
                Order order = _listings[i].order;
                uint amountRequested = _amountsRequestedByOrderId[order.orderId];
                total += order.priceUSD * amountRequested;
            }
            
            return total.ToString("F2");
        }

        public async Task<string> GetApproximateTotalInCurrency(Address currencyAddress)
        {
            int listings = _listings.Length;
            double total = 0;
            for (int i = 0; i < listings; i++)
            {
                Order order = _listings[i].order;
                uint amountRequested = _amountsRequestedByOrderId[order.orderId];
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
            
            return total.ToString("F99").TrimEnd('0').TrimEnd('.');
        }

        public async Task<Currency> GetBestCurrency()
        {
            if (_currencies == null)
            {
                _currencies = await _marketplaceReader.ListCurrencies();
            }
            
            Currency defaultChainCurrency = _currencies.FindDefaultChainCurrency();
            string chosenAddress = Currency.NativeCurrencyAddress;
            if (defaultChainCurrency != null)
            {
                chosenAddress = defaultChainCurrency.contractAddress;
            }
            int listings = _listings.Length;
            Dictionary<string, int> currencyCounts = new Dictionary<string, int>();
            for (int i = 0; i < listings; i++)
            {
                if (_listings[i].order.priceCurrencyAddress == chosenAddress)
                {
                    return _currencies.GetCurrencyByContractAddress(chosenAddress);
                }
                string currencyAddress = _listings[i].order.priceCurrencyAddress;
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

        public CollectibleOrder[] GetListings()
        {
            return _listings;
        }

        public IWallet GetWallet()
        {
            return _wallet;
        }

        public Dictionary<string, Sprite> GetCollectibleImagesByOrderId()
        {
            return _collectibleImagesByOrderId;
        }

        public Dictionary<string, uint> GetAmountsRequestedByOrderId()
        {
            return _amountsRequestedByOrderId;
        }

        public void SetAmountRequested(string orderId, uint amount)
        {
            if (!_amountsRequestedByOrderId.ContainsKey(orderId))
            {
                throw new ArgumentException(
                    "Invalid use. Cannot set amount requested for an order that is not in the cart.");
            }
            _amountsRequestedByOrderId[orderId] = amount;
        }

        public ICheckout GetICheckout()
        {
            return _checkout;
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
                    GetTokenBalancesReturn balancesReturn = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_wallet.GetWalletAddress(), currencyContractAddress));
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
                    int decimals = 18;
                    if (balance.contractInfo == null)
                    {
                        Debug.LogWarning($"No contract info found for {balance.contractAddress}, using default decimals of 18");
                    }
                    else
                    {
                        decimals = balance.contractInfo.decimals;
                    }

                    double balanceAmount = DecimalNormalizer.ReturnToNormal(balance.balance, decimals);
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
                    EtherBalance balancesReturn = await _indexer.GetEtherBalance(_wallet.GetWalletAddress());
                    if (balancesReturn == null)
                    {
                        throw new Exception("Received unexpected null response from indexer");
                    }
                    BigInteger balance = balancesReturn.balanceWei;
                    double balanceAmount = DecimalNormalizer.ReturnToNormal(balance,1);
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

        public async Task<TransactionReturn> Checkout()
        {
            Transaction[] transactions;
            try
            {
                transactions =  await BuildCheckoutTransactionArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Error building checkout transaction array: {e.Message}");
            }

            try
            {
                TransactionReturn transactionReturn = await _wallet.SendTransaction(_chain, transactions);
                return transactionReturn;
            }
            catch (Exception e)
            {
                throw new Exception($"Error sending checkout transactions: {e.Message}");
            }
        }

        private async Task<Transaction[]> BuildCheckoutTransactionArray()
        {
            int listings = _listings.Length;
            List<Transaction> transactions = new List<Transaction>();
            for (int i = 0; i < listings; i++)
            {
                CollectibleOrder listing = _listings[i];
                if (listing.order.priceCurrencyAddress == _chosenCurrency.contractAddress)
                {
                    Step[] steps = await _checkout.GenerateBuyTransaction(listing.order, _amountsRequestedByOrderId[listing.order.orderId]);
                    transactions.AddRange(steps.AsTransactionArray());
                }
                else
                {
                    SwapQuote quote = await _swap.GetSwapQuote(_wallet.GetWalletAddress(),
                        new Address(listing.order.priceCurrencyAddress),
                        new Address(_chosenCurrency.contractAddress),
                        listing.order.priceAmount, true);
                    transactions.AddRange(quote.AsTransactionArray());
                }
            }
            
            return transactions.ToArray();
        }
    }
}