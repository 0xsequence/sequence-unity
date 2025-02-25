using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class NftCheckout : ICheckoutHelper // Todo add an additional interface that handles the credit card based checkout
    {
        private IWallet _wallet;
        private CollectibleOrder _listing;
        private Order[] _listings;
        private Sprite _collectibleImage;
        private ulong _amountRequested;
        private ISwap _swap;
        private IMarketplaceReader _marketplaceReader;
        private IIndexer _indexer;
        private ICheckout _checkout;
        private Currency[] _currencies;
        private Dictionary<string, Sprite> _currencyIcons = new Dictionary<string, Sprite>();
        private Chain _chain;
        private Currency _chosenCurrency;
        private Page _page;
        private Dictionary<Order, ulong> _amountsByOrder;
        private IEthClient _client;

        public NftCheckout(IWallet wallet, CollectibleOrder listing, Sprite collectibleIcon, ulong amount, ISwap swap = null, IMarketplaceReader marketplaceReader = null, IIndexer indexer = null, ICheckout checkout = null, IEthClient client = null)
        {
            _wallet = wallet;
            _listing = listing;
            _collectibleImage = collectibleIcon;
            _amountRequested = amount;
            
            Setup(swap, marketplaceReader, indexer, checkout, client);
        }
        
        private void Setup(ISwap swap, IMarketplaceReader marketplaceReader, IIndexer indexer, ICheckout checkout, IEthClient client)
        {
            _chain = ChainDictionaries.ChainById[_listing.order.chainId.ToString()];
            SetSwap(swap);
            SetReader(marketplaceReader);
            SetIndexer(indexer);
            SetCheckout(checkout);
            SetClient(client);

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
            FetchOtherListings().ConfigureAwait(false);
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

        private void SetClient(IEthClient client)
        {
            _client = client;
            if (_client == null)
            {
                _client = new SequenceEthClient(_chain);
            }
        }
        
        private async Task FetchCurrencies()
        {
            _currencies = await _marketplaceReader.ListCurrencies();
            _chosenCurrency = _currencies.FindDefaultChainCurrency();
        }

        private async Task FetchOtherListings()
        {
            ListCollectibleListingsReturn result =
                await _marketplaceReader.ListListingsForCollectible(
                    new Address(_listing.order.collectionContractAddress), _listing.order.tokenId,
                    new OrderFilter(null, null, new[] { _listing.order.priceCurrencyAddress }));
            _page = result.page;
            _listings = result.listings;
            _listings = _listings.OrderBy(listing => listing.priceUSD).ToArray(); // Todo confirm if this comes pre-sorted by the API

            ulong remaining = await SetAmountsByOrder();
            if (remaining > 0)
            {
                ulong requested = _amountRequested + remaining; // We should already revert _amountRequested to the max available in SetAmountsByOrder
                Debug.LogError($"Amount requested exceeds what is available in the marketplace for collectible (contract: {_listing.order.collectionContractAddress}, tokenId: {_listing.order.tokenId}), amount requested: {requested} available: {_amountRequested}. Setting requested amount to the available amount: {_amountRequested}");
            }
        }

        private async Task<ulong> SetAmountsByOrder()
        {
            _amountsByOrder = new Dictionary<Order, ulong>();
            ulong remaining = _amountRequested;
            foreach (Order order in _listings)
            {
                ulong amount = Math.Min(remaining, ulong.Parse(order.quantityRemaining));
                _amountsByOrder[order] = amount;
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

        public string GetApproximateTotalInUSD()
        {
            decimal total = 0;
            foreach (var order in _amountsByOrder.Keys)
            {
                total = order.priceUSD * _amountsByOrder[order];
            }
            
            return total.ToString("F2");
        }

        public async Task<string> GetApproximateTotalInCurrency(Address currencyAddress)
        {
            decimal total = 0;
            ulong amountRemaining = _amountRequested;
            try
            {
                foreach (var order in _amountsByOrder.Keys)
                {
                    total += await GetApproximateTotalInCurrency(currencyAddress, amountRemaining, order);
                    amountRemaining -= _amountsByOrder[order];
                }
                return total.ToString("F99").TrimEnd('0').TrimEnd('.');
            }
            catch (Exception e)
            {
                return $"Error calculating approximate total in {currencyAddress}: {e.Message}";
            }
        }

        private async Task<decimal> GetApproximateTotalInCurrency(Address currencyAddress, ulong amountRemaining, Order order)
        {
            decimal total = 0;
            ulong amountRequested = amountRemaining;
            if (order.priceCurrencyAddress == currencyAddress)
            {
                total += DecimalNormalizer.ReturnToNormalPrecise(BigInteger.Parse(order.priceAmount), (int)order.priceDecimals) * amountRequested;
            }
            else
            {
                try
                {
                    SwapPrice price = await _swap.GetSwapPrice(currencyAddress, new Address(order.priceCurrencyAddress), order.priceAmount);
                    total += DecimalNormalizer.ReturnToNormalPrecise(BigInteger.Parse(price.maxPrice), (int)order.priceDecimals) * amountRequested;
                }
                catch (Exception e)
                {
                    string error =
                        $"Error fetching swap price for buying {order.priceAmount} of {order.priceCurrencyAddress} with {currencyAddress}: {e.Message}";
                    throw new Exception(error);
                }
            }
            
            return total;
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

        // Todo consider re-write such that we first check what currencies they have in their wallet before checking the swap price
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
                        Debug.LogWarning($"No contract info found for {balance.contractAddress}, attempting to fetch from contract...");
                        try
                        {
                            var decimalsFromContract = await new ERC20(currencyContractAddress).Decimals(_client);
                            decimals = (int)decimalsFromContract;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Error fetching decimals from contract for {currencyContractAddress}: {e.Message}\nUsing default of 18 decimals");
                        }
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

        public async Task<Currency> GetBestCurrency()
        {
            if (_currencies == null)
            {
                _currencies = await _marketplaceReader.ListCurrencies();
            }

            return _currencies.GetCurrencyByContractAddress(_listing.order.priceCurrencyAddress);
        }

        public CollectibleOrder[] GetListings()
        {
            return new CollectibleOrder[]
            {
                _listing
            };
        }

        public IWallet GetWallet()
        {
            return _wallet;
        }

        public Dictionary<Address, Dictionary<string, Sprite>> GetCollectibleImagesByCollectible()
        {
            return new Dictionary<Address, Dictionary<string, Sprite>>()
            {
                {
                    new Address(_listing.order.collectionContractAddress), new Dictionary<string, Sprite>()
                    {
                        { _listing.order.tokenId, _collectibleImage }
                    }
                }
            };
        }

        public Dictionary<Address, Dictionary<string, ulong>> GetAmountsRequestedByCollectible()
        {
            return new Dictionary<Address, Dictionary<string, ulong>>()
            {
                {
                    new Address(_listing.order.collectionContractAddress), new Dictionary<string, ulong>()
                    {
                        { _listing.order.tokenId, _amountRequested }
                    }
                }
            };
        }

        public Task<ulong> SetAmountRequested(Address collection, string tokenId, ulong amount)
        {
            if (collection.Value != _listing.order.collectionContractAddress || tokenId != _listing.order.tokenId)
            {
                throw new ArgumentException($"Invalid collectible: {collection}, {tokenId}, expected {_listing.order.collectionContractAddress}, {_listing.order.tokenId}");
            }
            _amountRequested = amount;
            return SetAmountsByOrder();
        }

        private async Task<Transaction[]> BuildCheckoutTransactionArray()
        {
            List<Transaction> transactions = new List<Transaction>();
            CollectibleOrder listing = _listing;
            if (listing.order.priceCurrencyAddress == _chosenCurrency.contractAddress)
            {
                Step[] steps = await _checkout.GenerateBuyTransaction(listing.order, _amountRequested);
                transactions.AddRange(steps.AsTransactionArray());
            }
            else
            {
                SwapQuote quote = await _swap.GetSwapQuote(_wallet.GetWalletAddress(),
                    new Address(listing.order.priceCurrencyAddress),
                    new Address(_chosenCurrency.contractAddress),
                    listing.order.priceAmount, true);
                transactions.AddRange(quote.AsTransactionArray());
                // Todo test this, I think I may be missing adding the buy transaction to the transactions list
            }
            
            return transactions.ToArray();
        }
    }
}