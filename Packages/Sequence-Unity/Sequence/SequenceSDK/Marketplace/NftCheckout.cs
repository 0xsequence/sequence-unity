using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
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
        private IEthClient _client;
        private OrderOrchestrator _orchestrator;

        public NftCheckout(IWallet wallet, CollectibleOrder listing, Sprite collectibleIcon, ulong amount, ISwap swap = null, IMarketplaceReader marketplaceReader = null, IIndexer indexer = null, ICheckout checkout = null, IEthClient client = null)
        {
            if (listing == null)
            {
                throw new ArgumentNullException($"{nameof(listing)}", $"Invalid use. {nameof(listing)} cannot be null");
            }
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

        private async Task FetchOtherListings()
        {
            _orchestrator = OrderOrchestrator.GetOrchestrator(_listing, _amountRequested, _marketplaceReader);
            await AsyncExtensions.WaitUntilConditionMet(IsInitialized);
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

        public async Task<string> GetApproximateTotalInUSD()
        {
            decimal total = 0;
            await AsyncExtensions.WaitUntilConditionMet(IsInitialized);
            foreach (var order in _orchestrator.AmountsByOrder.Keys)
            {
                total = order.priceUSD * _orchestrator.AmountsByOrder[order];
            }
            
            return total.ToString("F2");
        }

        private bool IsInitialized()
        {
            if (_orchestrator == null)
            {
                return false;
            }
            return _orchestrator.Initialized;
        }

        public async Task<string> GetApproximateTotalInCurrency(Address currencyAddress)
        {
            decimal total = 0;
            ulong amountRemaining = _amountRequested;
            try
            {
                await AsyncExtensions.WaitUntilConditionMet(IsInitialized);
                foreach (var order in _orchestrator.AmountsByOrder.Keys)
                {
                    total += await GetApproximateTotalInCurrency(currencyAddress, amountRemaining, order);
                    amountRemaining -= _orchestrator.AmountsByOrder[order];
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

        public Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress, TokenBalance userBalance = null)
        {
            return new AffordabilityCalculator(GetApproximateTotalInCurrency, _indexer, _wallet, _client, _chain).GetApproximateTotalInCurrencyIfAffordable(currencyContractAddress, userBalance);
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

        public CartItemData[] GetCartItemData()
        {
            return new CartItemData[]
            {
                new CartItemData(_listing.metadata.name, _listing.order.tokenId, new Address(_listing.order.collectionContractAddress), _chain)
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
            return _orchestrator.SetAmount(_amountRequested);
        }

        private async Task<Transaction[]> BuildCheckoutTransactionArray()
        {
            List<Transaction> transactions = new List<Transaction>();
            CollectibleOrder listing = _listing;
            if (listing.order.priceCurrencyAddress != _chosenCurrency.contractAddress)
            {
                SwapQuote quote = await _swap.GetSwapQuote(_wallet.GetWalletAddress(),
                    new Address(listing.order.priceCurrencyAddress),
                    new Address(_chosenCurrency.contractAddress),
                    listing.order.priceAmount, true);
                transactions.AddRange(quote.AsTransactionArray());
            }
            Step[] steps = await _checkout.GenerateBuyTransaction(listing.order, _amountRequested);
            transactions.AddRange(steps.AsTransactionArray());
            
            return transactions.ToArray();
        }
    }
}