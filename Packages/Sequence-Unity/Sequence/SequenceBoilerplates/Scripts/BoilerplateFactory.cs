using System;
using System.Collections.Generic;
using Sequence.Boilerplates.DailyRewards;
using Sequence.Boilerplates.InGameShop;
using Sequence.Boilerplates.Inventory;
using Sequence.Boilerplates.Login;
using Sequence.Boilerplates.PlayerProfile;
using Sequence.Boilerplates.SignMessage;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sequence.Boilerplates
{
    public static class BoilerplateFactory
    {
        private static Dictionary<Type, GameObject> _objects = new();

        /// <summary>
        /// Call this function to clear references in memory when objects are destroyed. For example, when loading a new scene.
        /// </summary>
        public static void CleanUp()
        {
            _objects.Clear();
        }
        
        /// <summary>
        /// Open the Login UI Boilerplate from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">Wallet to use for account federation.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window or when an account was successfully federated.</param>
        /// <returns></returns>
        public static SequenceLoginWindow OpenSequenceLoginWindow(Transform parent, IWallet wallet = null, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceLoginWindow>("Login/SequenceLoginWindow", parent, 
                b => b.Show(wallet, onClose));
        }

        /// <summary>
        /// Open the Player Profile UI Boilerplate from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="currency">Define a custom ERC20 currency. Leave it null to use the chains native token.</param>
        /// <param name="currencySymbol">The symbol of your custom currency, such as 'ETH'.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <returns>Instance of SequencePlayerProfile which was instantiated as a child of <paramref name="parent"/></returns>
        public static SequencePlayerProfile OpenSequencePlayerProfile(Transform parent, IWallet wallet, Chain chain, Address currency = null, string currencySymbol = null, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequencePlayerProfile>("PlayerProfile/SequencePlayerProfile", parent, 
                b => b.Show(wallet, chain, currency, currencySymbol, onClose));
        }

        /// <summary>
        /// Open the Daily Rewards UI Boilerplate from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="apiUrl">API Url you deployed using the server boilerplate.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <returns>Instance of SequenceDailyRewards which was instantiated as a child of <paramref name="parent"/></returns>
        public static SequenceDailyRewards OpenSequenceDailyRewards(Transform parent, IWallet wallet, Chain chain, string apiUrl, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceDailyRewards>("DailyRewards/SequenceDailyRewards", parent, 
                b => b.Show(wallet, chain, apiUrl, onClose));
        }
        
        /// <summary>
        /// Open the Inventory UI Boilerplate from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="collections">The inventory will show items from these contracts.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <returns>Instance of SequenceInventory which was instantiated as a child of <paramref name="parent"/></returns>
        public static SequenceInventory OpenSequenceInventory(Transform parent, IWallet wallet, Chain chain, string[] collections, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceInventory>("Inventory/SequenceInventory", parent, 
                b => b.Show(wallet, chain, collections, onClose));
        }
        
        /// <summary>
        /// Open the In-Game Shop UI Boilerplate from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="tokenContractAddress">ERC1155 Contract you deployed on Sequence's Builder.</param>
        /// <param name="saleContractAddress">ERC1155 Sale Contract you deployed on Sequence's Builder.</param>
        /// <param name="itemsForSale">Define the token Ids you want to sell from your collection.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <returns>Instance of SequenceInGameShop which was instantiated as a child of <paramref name="parent"/></returns>
        public static SequenceInGameShop OpenSequenceInGameShop(Transform parent, IWallet wallet, Chain chain, 
            string tokenContractAddress, string saleContractAddress, int[] itemsForSale, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceInGameShop>("InGameShop/SequenceInGameShop", parent, 
                b => b.Show(wallet, chain, tokenContractAddress, saleContractAddress, itemsForSale, onClose));
        }
        
        /// <summary>
        /// Opens the view marketplace listings boilerplate from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <param name="marketplaceCollectionAddress">The initial collection address to show when opening the window.</param>
        /// <returns>Instance of ViewMarketplaceListingsPanel which was instantiated as a child of <paramref name="parent"/></returns>
        public static ViewMarketplaceListingsPage OpenViewMarketplaceListingsPanel(Transform parent, IWallet wallet, Chain chain, Address marketplaceCollectionAddress, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<ViewMarketplaceListingsPage>("Marketplace/ViewMarketplaceListingsPanel", parent, 
                b => b.Show(wallet, chain, marketplaceCollectionAddress, onClose));
        }

        /// <summary>
        /// Open the checkout panel for crypto checkout with swaps or credit card based checkout
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="checkoutHelper">An implementation of the ICheckoutHelper interface.</param>
        /// <param name="fiatCheckout">An implementation of the IFiatCheckout interface.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <returns></returns>
        public static CheckoutPage OpenCheckoutPanel(Transform parent, Chain chain, ICheckoutHelper checkoutHelper,
            IFiatCheckout fiatCheckout, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<CheckoutPage>("Checkout/CheckoutPanel", parent,
                b => b.Show(chain, checkoutHelper, fiatCheckout, onClose));
        }

        public static ListItemPage OpenListItemPanel(Transform parent, ICheckout checkout, TokenBalance item, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<ListItemPage>("Checkout/ListItemPanel", parent, b => b.Open(checkout, item));
        }



        public static CreateOfferPage OpenCreateOfferPanel(Transform parent, ICheckout checkout, TokenBalance item, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<CreateOfferPage>("Checkout/CreateOfferPanel", parent, b => b.Open(checkout, item));
        }
        public static CreateOfferPage OpenCreateOfferPanel(Transform parent, ICheckout checkout, CollectibleOrder item, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<CreateOfferPage>("Checkout/CreateOfferPanel", parent, b => b.Open(checkout, item));
        }

        public static SellOfferPage OpenSellOfferPanel(Transform parent, ICheckout checkout, CollectibleOrder item, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SellOfferPage>("Checkout/SellOfferPanel", parent, b => b.Open(checkout, item));
        }
        /// <summary>
        /// Open the UI Boilerplate to sign messages from a Prefab inside the Resources folder.
        /// </summary>
        /// <param name="parent">Transform inside of a Canvas object.</param>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        /// <returns>Instance of SequenceSignMessage which was instantiated as a child of <paramref name="parent"/></returns>
        public static SequenceSignMessage OpenSequenceSignMessage(Transform parent, IWallet wallet, Chain chain, Action onClose = null)
        {
            return GetOrSpawnBoilerplate<SequenceSignMessage>("SignMessage/SequenceSignMessage", parent, 
                b => b.Show(wallet, chain, onClose));
        }
        
        private static T GetOrSpawnBoilerplate<T>(string path, Transform parent, Action<T> show) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_objects.ContainsKey(type))
            {
                var cachedBoilerplate = _objects[type].GetComponent<T>();
                show.Invoke(cachedBoilerplate);
                return cachedBoilerplate;
            }
            
            var prefab = ((GameObject)Resources.Load($"Prefabs/{path}")).GetComponent<T>();
            if (prefab == null)
                throw new Exception($"Prefab at {path} not found in Resources folder");
            
            var boilerplate = Object.Instantiate(prefab, parent);
            _objects.Add(type, boilerplate.gameObject);
            show.Invoke(boilerplate);
            return boilerplate;
        }
    }
}