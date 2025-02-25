using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Utils;

namespace Sequence.Demo.Mocks
{
    /// <summary>
    /// This mock implementation of ICheckout uses Checkout to implement all methods. Additionally, it will append Sardine as a CheckoutOption if not there - useful for people who live in Canada (or other countries that Sardine doesn't support)
    /// </summary>
    public class MockCheckoutWithSardineForCanada : ICheckout
    {
        private Checkout _checkout;

        public MockCheckoutWithSardineForCanada(Checkout checkout)
        {
            _checkout = checkout;
        }

        public event Action<Marketplace.CheckoutOptions> OnCheckoutOptionsReturn;
        public event Action<string> OnCheckoutOptionsError;
        public async Task<Marketplace.CheckoutOptions> GetCheckoutOptions(CheckoutOptionsMarketplaceOrder[] orders, int additionalFeeBps = 0)
        {
            Marketplace.CheckoutOptions options = await _checkout.GetCheckoutOptions(orders, additionalFeeBps);
            return AppendSardine(options);
        }

        private Marketplace.CheckoutOptions AppendSardine(Marketplace.CheckoutOptions options)
        {
            if (options != null)
            {
                if (options.onRamp == null)
                {
                    options.onRamp = new TransactionOnRampProvider[] { TransactionOnRampProvider.sardine };
                }
                else if (!options.onRamp.Contains(TransactionOnRampProvider.sardine))
                {
                    options.onRamp = options.onRamp.AppendObject(TransactionOnRampProvider.sardine);
                }

                if (options.nftCheckout == null)
                {
                    options.nftCheckout = new TransactionNFTCheckoutProvider[]
                    {
                        TransactionNFTCheckoutProvider.sardine
                    };
                }
                else if (!options.nftCheckout.Contains(TransactionNFTCheckoutProvider.sardine))
                {
                    options.nftCheckout = options.nftCheckout.AppendObject(TransactionNFTCheckoutProvider.sardine);
                }
            }

            return options;
        }

        public async Task<Marketplace.CheckoutOptions> GetCheckoutOptions(Order[] orders, int additionalFeeBps = 0)
        {
            var options = await _checkout.GetCheckoutOptions(orders, additionalFeeBps);
            return AppendSardine(options);
        }

        public async Task<Marketplace.CheckoutOptions> GetCheckoutOptions(ERC1155Sale saleContract, Address collection, Dictionary<string, BigInteger> amountsByTokenId)
        {
            var options = await _checkout.GetCheckoutOptions(saleContract, collection, amountsByTokenId);
            return AppendSardine(options);
        }

        public async Task<Marketplace.CheckoutOptions> GetCheckoutOptions(ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount)
        {
            var options = await _checkout.GetCheckoutOptions(saleContract, collection, tokenId, amount);
            return AppendSardine(options);
        }

        public event Action<Step[]> OnTransactionStepsReturn;
        public event Action<string> OnTransactionStepsError;
        public Task<Step[]> GenerateBuyTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null, Address buyer = null)
        {
            return _checkout.GenerateBuyTransaction(order, amount, additionalFee, buyer);
        }

        public Task<Step[]> GenerateBuyTransaction(Order[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address buyer = null)
        {
            return _checkout.GenerateBuyTransaction(orders, amount, additionalFee, buyer);
        }

        public Task<Step[]> GenerateSellTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null, Address seller = null)
        {
            return _checkout.GenerateSellTransaction(order, amount, additionalFee, seller);
        }

        public Task<Step[]> GenerateSellTransaction(Order[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address seller = null)
        {
            return _checkout.GenerateSellTransaction(orders, amount, additionalFee, seller);
        }

        public Task<Step[]> GenerateBuyTransaction(CollectibleOrder[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address buyer = null)
        {
            return _checkout.GenerateBuyTransaction(orders, amount, additionalFee, buyer);
        }

        public Task<Step[]> GenerateSellTransaction(CollectibleOrder[] orders, BigInteger amount, AdditionalFee[] additionalFee = null,
            Address seller = null)
        {
            return _checkout.GenerateSellTransaction(orders, amount, additionalFee, seller);
        }

        public Task<Step[]> GenerateListingTransaction(Address collection, string tokenId, BigInteger amount, Marketplace.ContractType contractType,
            Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry,
            OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2)
        {
            return _checkout.GenerateListingTransaction(collection, tokenId, amount, contractType, currencyTokenAddress, pricePerToken, expiry, orderbookKind);
        }

        public Task<Step[]> GenerateOfferTransaction(Address collection, string tokenId, BigInteger amount, Marketplace.ContractType contractType,
            Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry,
            OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2)
        {
            return _checkout.GenerateOfferTransaction(collection, tokenId, amount, contractType, currencyTokenAddress, pricePerToken, expiry, orderbookKind);
        }

        public Task<Step[]> GenerateCancelTransaction(Address collection, string orderId,
            MarketplaceKind marketplaceKind = MarketplaceKind.sequence_marketplace_v2)
        {
            return _checkout.GenerateCancelTransaction(collection, orderId, marketplaceKind);
        }
    }
}