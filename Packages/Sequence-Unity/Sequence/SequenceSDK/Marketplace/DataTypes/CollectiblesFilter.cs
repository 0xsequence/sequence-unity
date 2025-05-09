using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Preserve]
    [Serializable]
    public class CollectiblesFilter
    {
        public bool includeEmpty;
        public string searchText = "";
        public PropertyFilter[] properties = null;
        public string[] marketplaces = null;
        public string[] inAccounts = null; // Filter for collectibles owned by the specified addresses
        public string[] notInAccounts = null; // Filter out any collectibles owned by the specified addresses
        public string[] ordersCreatedBy = null; // Filter for orders created by the specified addresses
        public string[] ordersNotCreatedBy = null; // Filter out any orders created by the specified addresses
        public string[] inCurrencyAddresses = null; // Filter for collectibles priced with the specified currency addresses
        public string[] notInCurrencyAddresses = null; // Filter out any collectibles priced with the specified currency addresses
        
        public CollectiblesFilter(bool includeEmpty, string searchText = "", PropertyFilter[] properties = null, MarketplaceKind[] marketplaces = null, string[] inAccounts = null, string[] notInAccounts = null, string[] ordersCreatedBy = null, string[] ordersNotCreatedBy = null, string[] inCurrencyAddresses = null, string[] notInCurrencyAddresses = null)
        {
            this.includeEmpty = includeEmpty;
            this.searchText = searchText;
            this.properties = properties;
            this.marketplaces = MarketplacesToStringArray(marketplaces);
            this.inAccounts = inAccounts;
            this.notInAccounts = notInAccounts;
            this.ordersCreatedBy = ordersCreatedBy;
            this.ordersNotCreatedBy = ordersNotCreatedBy;
            this.inCurrencyAddresses = inCurrencyAddresses;
            this.notInCurrencyAddresses = notInCurrencyAddresses;
        }

        [Preserve]
        [JsonConstructor]
        public CollectiblesFilter(bool includeEmpty, string searchText, PropertyFilter[] properties, string[] marketplaces, string[] inAccounts, string[] notInAccounts, string[] ordersCreatedBy, string[] ordersNotCreatedBy, string[] inCurrencyAddresses = null, string[] notInCurrencyAddresses = null)
        {
            this.includeEmpty = includeEmpty;
            this.searchText = searchText;
            this.properties = properties;
            this.marketplaces = marketplaces;
            this.inAccounts = inAccounts;
            this.notInAccounts = notInAccounts;
            this.ordersCreatedBy = ordersCreatedBy;
            this.ordersNotCreatedBy = ordersNotCreatedBy;
            this.inCurrencyAddresses = inCurrencyAddresses;
            this.notInCurrencyAddresses = notInCurrencyAddresses;
        }

        private string[] MarketplacesToStringArray(MarketplaceKind[] marketplaces)
        {
            if (marketplaces == null)
            {
                return null;
            }
            int length = marketplaces.Length;
            string[] marketplacesString = new string[length];
            for (int i = 0; i < length; i++)
            {
                marketplacesString[i] = marketplaces[i].ToString();
            }

            return marketplacesString;
        }
    }
}