using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.Marketplace
{
    public interface ICheckoutHelper
    {
        public static event Action<Marketplace.Currency> OnSelectedCurrency;

        /// <summary>
        /// Emit a static event indicating that a currency has been selected, most likely from the UI
        /// </summary>
        /// <param name="currency"></param>
        public static void SelectCurrency(Currency currency)
        {
            OnSelectedCurrency?.Invoke(currency);
        }

        /// <summary>
        /// Get the approximate total cost to checkout in USD as a string
        /// </summary>
        /// <returns></returns>
        public Task<string> GetApproximateTotalInUSD();
        
        /// <summary>
        /// Get the approximate total cost to checkout using the specified currencyAddress as a string in human-readable format
        /// </summary>
        /// <param name="currencyAddress"></param>
        /// <returns></returns>
        public Task<string> GetApproximateTotalInCurrency(Address currencyAddress);
        
        /// <summary>
        /// Get an array of all whitelisted Currencies on the current chain
        /// </summary>
        /// <returns></returns>
        public Task<Currency[]> GetCurrencies();
        
        /// <summary>
        /// Get the icon, as a sprite, for the provided Currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Task<Sprite> GetCurrencyIcon(Currency currency);
        
        /// <summary>
        /// Similar to GetApproximateTotalInCurrency, return the approximate total cost in currencyContractAddress as a string in human-readable format
        /// Except, if the user doesn't have sufficient balance of currencyContractAddress, then "" is returned
        /// </summary>
        /// <param name="currencyContractAddress"></param>
        /// <param name="userBalance">If known, provide the user's balance in the currency</param>
        /// <returns></returns>
        public Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress, TokenBalance userBalance = null);
        
        /// <summary>
        /// Purchase all the listings using the previously selected Currency, making swaps when necessary
        /// All transactions are submitted in a batch
        /// </summary>
        /// <returns></returns>
        public Task<TransactionReturn> Checkout();
        
        /// <summary>
        /// Get the "best" currency for pricing the transaction in; this is likely the chain's default currency or the most commonly used currency
        /// This can be useful for displaying a price in UIs
        /// </summary>
        /// <returns></returns>
        public Task<Currency> GetBestCurrency();
        
        /// <summary>
        /// Get the CartItemData[] of collectibles the user is looking to buy (that have been added to the ICheckoutHelper)
        /// </summary>
        /// <returns></returns>
        public CartItemData[] GetCartItemData();
        
        /// <summary>
        /// Get the wallet that will be making the purchases
        /// </summary>
        /// <returns></returns>
        public IWallet GetWallet();

        /// <summary>
        /// Get the collectible icons/images organized in a dictionary with their associated collection Address and token id as keys
        /// </summary>
        /// <returns></returns>
        Dictionary<Address, Dictionary<string, Sprite>> GetCollectibleImagesByCollectible();

        /// <summary>
        /// Get the amounts of collectibles to purchase organized in a dictionary with their associated collection Address and token id as keys
        /// </summary>
        /// <returns></returns>
        public Dictionary<Address, Dictionary<string, ulong>> GetAmountsRequestedByCollectible();

        /// <summary>
        /// Set the amount of a given collectible (identified by collection address and token id) the user wishes to purchase
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <returns>Any amount requested that exceeds what is available or 0 if requested amount is fulfill-able. e.g. if there are 10 available and amount is 12, 2 would be returned</returns>
        public Task<ulong> SetAmountRequested(Address collection, string tokenId, ulong amount);
    }
}