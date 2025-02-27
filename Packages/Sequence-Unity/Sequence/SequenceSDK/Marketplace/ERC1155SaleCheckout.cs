using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    public class ERC1155SaleCheckout : ICheckoutHelper
    {
        private ERC1155Sale _sale;
        private ERC1155 _collection;
        private Address _paymentToken;
        private string _tokenId;
        private ulong _amount;
        private ERC1155Sale.SaleDetails _saleDetails;
        private Chain _chain;
        private IWallet _wallet;
        private ISwap _swap;
        private IEthClient _ethClient;
        private string _paymentTokenIconUrl;
        private Dictionary<Address, Sprite> _iconsByAddress;
        private Address _chosenCurrency;
        private string _saleName;
        private Sprite _collectibleImage;
        private IIndexer _indexer;

        // Override default constructor so we can't create an instance without using the Create method - ensuring we have appropriate data
        public ERC1155SaleCheckout()
        {
            throw new NotSupportedException("ERC1155SaleCheckout must be created using the Create method");
        }

        public static Task<ERC1155SaleCheckout> Create(ERC1155Sale sale, ERC1155 collection, string tokenId, ulong amount, Chain chain,
            IWallet wallet, string saleName, string paymentTokenIconUrl, Sprite collectibleImage, ISwap swap = null,
            IEthClient client = null, IIndexer indexer = null)
        {
            return new ERC1155SaleCheckout(sale).Assemble(sale, collection, tokenId, amount, chain, wallet, saleName,
                paymentTokenIconUrl, collectibleImage, swap, client, indexer);
        }

        private ERC1155SaleCheckout(ERC1155Sale sale)
        {
            _sale = sale;
        }

        private async Task<ERC1155SaleCheckout> Assemble(ERC1155Sale sale, ERC1155 collection, string tokenId, ulong amount, Chain chain,
            IWallet wallet, string saleName, string paymentTokenIconUrl, Sprite collectibleImage, ISwap swap = null,
            IEthClient client = null, IIndexer indexer = null)
        {
            _sale = sale;
            _collection = collection;
            _ethClient = client;
            _chain = chain;
            if (_ethClient == null)
            {
                _ethClient = new SequenceEthClient(_chain);
            }
            _paymentToken = await sale.GetPaymentTokenAsync(_ethClient);
            _tokenId = tokenId;
            _amount = amount;
            _saleDetails = await sale.TokenSaleDetailsAsync(_ethClient, BigInteger.Parse(_tokenId));
            _wallet = wallet;
            _swap = swap;
            if (_swap == null)
            {
                _swap = new CurrencySwap(_chain);
            }
            _paymentTokenIconUrl = paymentTokenIconUrl;
            _iconsByAddress = new Dictionary<Address, Sprite>();
            _chosenCurrency = _paymentToken;
            _saleName = saleName;
            _collectibleImage = collectibleImage;
            _indexer = indexer;
            if (_indexer == null)
            {
                _indexer = new ChainIndexer(_chain);
            }

            return this;
        }

        public async Task<string> GetApproximateTotalInUSD()
        {
            try
            {
                Currency bestCurrency = await GetBestCurrency();
                decimal approximateTotalInCurrency =
                    await GetApproximateTotalInCurrencyAsDecimal(new Address(bestCurrency.contractAddress));
                TokenPrice[] prices =
                    await new PriceFeed().GetCoinPrices(new PriceFeed.Token(_chain,
                        new Address(bestCurrency.contractAddress)));

                if (prices != null && prices.Length > 0)
                {
                    return (approximateTotalInCurrency * prices[0].price.value).ToString("F2");
                }
                else
                {
                    throw new Exception(
                        $"API returned a null {typeof(TokenPrice)} response for currency token address {bestCurrency.contractAddress}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Encountered error fetching approximate total in USD: " + e.Message);
                return "";
            }
        }

        public async Task<string> GetApproximateTotalInCurrency(Address currencyAddress)
        {
            try
            {
                decimal total = await GetApproximateTotalInCurrencyAsDecimal(currencyAddress);
                return total.ToString("F99").TrimEnd('0').TrimEnd('.');
            }
            catch (Exception e)
            {
                string error = $"Error fetching approximate total in currency {currencyAddress} for buying {_amount} of tokenIf {_tokenId} from collection {_collection} using sale contract {_sale.Contract.GetAddress()} on chain {_chain}: {e.Message}";
                throw new Exception(error);
            }
        }

        private async Task<decimal> GetApproximateTotalInCurrencyAsDecimal(Address currencyAddress)
        {
            BigInteger total = _saleDetails.Cost * _amount;
            if (!currencyAddress.Equals(_paymentToken))
            {
                try
                {
                    SwapPrice swapPrice = await _swap.GetSwapPrice(_paymentToken, currencyAddress, (_saleDetails.Cost * _amount).ToString());
                    total = BigInteger.Parse(swapPrice.maxPrice);
                }
                catch (Exception e)
                {
                    string error =
                        $"Error fetching swap price for buying {total} of {_paymentToken} with {currencyAddress}: {e.Message}";
                    throw new Exception(error);
                }
            }
            
            ERC20 currency = new ERC20(currencyAddress);
            BigInteger decimals = 18;
            try
            {
                decimals = await currency.Decimals(_ethClient);
            }
            catch (Exception e)
            {
                string error = $"Error fetching decimals for {currencyAddress}: {e.Message}";
                throw new Exception(error);
            }

            return DecimalNormalizer.ReturnToNormalPrecise(total, (int)decimals);
        }

        public async Task<Currency[]> GetCurrencies()
        {
            Currency paymentCurrency = await BuildCurrency(new ERC20(_paymentToken));
            return new[] { paymentCurrency };
        }

        private async Task<Currency> BuildCurrency(ERC20 currency)
        {
            string name = await currency.Name(_ethClient);
            string symbol = await currency.Symbol(_ethClient);
            BigInteger decimals = await currency.Decimals(_ethClient);
            
            return new Currency(0, _chain, currency.GetAddress(), name, symbol, (ulong)decimals, _paymentTokenIconUrl, 0, true, "", "");
        }

        public async Task<Sprite> GetCurrencyIcon(Currency currency)
        {
            Address currencyAddress = new Address(currency.contractAddress);
            if (_iconsByAddress.TryGetValue(currencyAddress, out var icon))
            {
                return icon;
            }
            
            _iconsByAddress[currencyAddress] = await AssetHandler.GetSpriteAsync(currency.imageUrl);
            return _iconsByAddress[currencyAddress];
        }

        public Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress)
        {
            return new AffordabilityCalculator(GetApproximateTotalInCurrency, _indexer, _wallet, _ethClient, _chain).GetApproximateTotalInCurrencyIfAffordable(currencyContractAddress);
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
            List<Transaction> transactions = new List<Transaction>();
            if (!_paymentToken.Equals(_chosenCurrency))
            {
                SwapQuote quote = await _swap.GetSwapQuote(_wallet.GetWalletAddress(), _paymentToken, _chosenCurrency,
                    (_saleDetails.Cost * _amount).ToString(), true);
                transactions.AddRange(quote.AsTransactionArray());
            }

            transactions.Add(new RawTransaction(_sale.Mint(_wallet.GetWalletAddress(),
                new BigInteger[] { BigInteger.Parse(_tokenId) },
                new BigInteger[] { BigInteger.Parse(_amount.ToString()) }, null, _paymentToken,
                _saleDetails.Cost * _amount)));
            
            return transactions.ToArray();
        }

        public Task<Currency> GetBestCurrency()
        {
            return BuildCurrency(new ERC20(_paymentToken));
        }

        public CartItemData[] GetCartItemData()
        {
            return new CartItemData[]
            {
                new CartItemData(_saleName, _tokenId, _collection, _chain)
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
                    _collection, new Dictionary<string, Sprite>()
                    {
                        { _tokenId, _collectibleImage }
                    }
                }
            };
        }

        public Dictionary<Address, Dictionary<string, ulong>> GetAmountsRequestedByCollectible()
        {
            return new Dictionary<Address, Dictionary<string, ulong>>()
            {
                {
                    _collection, new Dictionary<string, ulong>()
                    {
                        { _tokenId, _amount }
                    }
                }
            };
        }

        public async Task<ulong> SetAmountRequested(Address collection, string tokenId, ulong amount)
        {
            if (!collection.Equals(_collection.Contract.GetAddress()) || tokenId != _tokenId)
            {
                throw new ArgumentException($"Invalid collectible: {collection}, {tokenId}, expected {_collection}, {_tokenId}");
            }

            _amount = amount;
            if (_amount > _saleDetails.SupplyCap)
            {
                _amount = (ulong)_saleDetails.SupplyCap;
                return amount - _amount;
            }

            return 0;
        }
    }
}