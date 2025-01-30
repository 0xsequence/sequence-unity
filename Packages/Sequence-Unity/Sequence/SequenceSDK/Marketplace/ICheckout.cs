using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;

namespace Sequence.Marketplace
{
    public interface ICheckout
    {
        public event Action<CheckoutOptions> OnCheckoutOptionsReturn;
        public event Action<string> OnCheckoutOptionsError;
        
        /// <summary>
        /// Get all the checkout options for a set of orders
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="additionalFeeBps"></param>
        /// <returns></returns>
        public Task<CheckoutOptions> GetCheckoutOptions(CheckoutOptionsMarketplaceOrder[] orders,
            int additionalFeeBps = 0);
       
        /// <summary>
        /// Get all the checkout options for a set of orders
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="additionalFeeBps"></param>
        /// <returns></returns>
        public Task<CheckoutOptions> GetCheckoutOptions(Order[] orders, int additionalFeeBps = 0);

        /// <summary>
        /// Get all the checkout options for a given ERC1155 sale contract, collection, and amounts
        /// </summary>
        /// <param name="saleContract"></param>
        /// <param name="collection"></param>
        /// <param name="amountsByTokenId"></param>
        /// <returns></returns>
        public Task<CheckoutOptions> GetCheckoutOptions(ERC1155Sale saleContract, Address collection,
            Dictionary<string, BigInteger> amountsByTokenId);

        /// <summary>
        /// Get all the checkout options for a given ERC721 sale contract, collection, tokenId, and amount
        /// </summary>
        /// <param name="saleContract"></param>
        /// <param name="collection"></param>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Task<CheckoutOptions> GetCheckoutOptions(ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount);
        
        public event Action<Step[]> OnTransactionStepsReturn;
        public event Action<string> OnTransactionStepsError;

        /// <summary>
        /// Get the Step[] that, when executed, will create a buy transaction for the given order and amount, fulfilling the order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="amount"></param>
        /// <param name="additionalFee">add an additional fee to be paid during the order fulfillment</param>
        /// <returns></returns>
        public Task<Step[]> GenerateBuyTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null);

        /// <summary>
        /// Get the Step[] that, when executed, will create a sell transaction for the given order and amount, fulfilling the order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="amount"></param>
        /// <param name="additionalFee">add an additional fee to be paid during the order fulfillment</param>
        /// <returns></returns>
        public Task<Step[]> GenerateSellTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null);

        /// <summary>
        /// Get the Step[] that, when executed, will create a new listing order
        /// </summary>
        /// <param name="collection">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="amount"></param>
        /// <param name="contractType">the collectible contract type</param>
        /// <param name="currencyTokenAddress">the Currency/ERC20 token address; note: must be whitelisted! If whitelisted, the Currency should be returned when using IMarketplaceReader.ListCurrencies()</param>
        /// <param name="pricePerToken">the price, in currencyTokenAddress, for each collectible in the order</param>
        /// <param name="expiry">order will be cancelled automatically past expiry DateTime</param>
        /// <param name="orderbookKind">the orderbook type for the order, defaults to OrderbookKind.sequence_marketplace_v2</param>
        /// <returns></returns>
        public Task<Step[]> GenerateListingTransaction(Address collection, string tokenId, BigInteger amount,
            ContractType contractType, Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry,
            OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2);

        /// <summary>
        /// Get the Step[] that, when executed, will create a new offer order
        /// </summary>
        /// <param name="collection">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="amount"></param>
        /// <param name="contractType">the collectible contract type</param>
        /// <param name="currencyTokenAddress">the Currency/ERC20 token address; note: must be whitelisted! If whitelisted, the Currency should be returned when using IMarketplaceReader.ListCurrencies()</param>
        /// <param name="pricePerToken">the price, in currencyTokenAddress, for each collectible in the order</param>
        /// <param name="expiry">order will be cancelled automatically past expiry DateTime</param>
        /// <param name="orderbookKind">the orderbook type for the order, defaults to OrderbookKind.sequence_marketplace_v2</param>
        /// <returns></returns>
        public Task<Step[]> GenerateOfferTransaction(Address collection, string tokenId, BigInteger amount,
            ContractType contractType, Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry,
            OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2);

        /// <summary>
        /// Get the Step[] that, when executed, will cancel an existing order
        /// </summary>
        /// <param name="collection">the collection contract address</param>
        /// <param name="orderId">the order id to be cancelled</param>
        /// <param name="marketplaceKind">the marketplace the order is on, defaults to MarketplaceKind.sequence_marketplace_v2</param>
        /// <returns></returns>
        public Task<Step[]> GenerateCancelTransaction(Address collection, string orderId,
            MarketplaceKind marketplaceKind = MarketplaceKind.sequence_marketplace_v2);
        
        /// <summary>
        /// Get the Step[] that, when executed, will cancel an existing order
        /// </summary>
        /// <param name="collection">the collection contract address</param>
        /// <param name="order">the order to be cancelled</param>
        /// <param name="marketplaceKind">the marketplace the order is on, defaults to MarketplaceKind.sequence_marketplace_v2</param>
        /// <returns></returns>
        public Task<Step[]> GenerateCancelTransaction(Address collection, Order order,
            MarketplaceKind marketplaceKind = MarketplaceKind.sequence_marketplace_v2)
        {
            return GenerateCancelTransaction(collection, order.orderId, marketplaceKind);
        }
    }
}