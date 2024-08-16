using System;
using Newtonsoft.Json;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CollectiblesFilter
    {
        public bool includeEmpty;
        public string searchText = "";
        public PropertyFilter[] properties = null;
        public string[] marketplaces = null;
        public string[] inAccounts = null;
        public string[] notInAccounts = null;
        public string[] ordersCreatedBy = null;
        public string[] ordersNotCreatedBy = null;
        
        public CollectiblesFilter(bool includeEmpty, string searchText = "", PropertyFilter[] properties = null, MarketplaceKind[] marketplaces = null, string[] inAccounts = null, string[] notInAccounts = null, string[] ordersCreatedBy = null, string[] ordersNotCreatedBy = null)
        {
            this.includeEmpty = includeEmpty;
            this.searchText = searchText;
            this.properties = properties;
            this.marketplaces = MarketplacesToStringArray(marketplaces);
            this.inAccounts = inAccounts;
            this.notInAccounts = notInAccounts;
            this.ordersCreatedBy = ordersCreatedBy;
            this.ordersNotCreatedBy = ordersNotCreatedBy;
        }

        [JsonConstructor]
        public CollectiblesFilter(bool includeEmpty, string searchText, PropertyFilter[] properties, string[] marketplaces, string[] inAccounts, string[] notInAccounts, string[] ordersCreatedBy, string[] ordersNotCreatedBy)
        {
            this.includeEmpty = includeEmpty;
            this.searchText = searchText;
            this.properties = properties;
            this.marketplaces = marketplaces;
            this.inAccounts = inAccounts;
            this.notInAccounts = notInAccounts;
            this.ordersCreatedBy = ordersCreatedBy;
            this.ordersNotCreatedBy = ordersNotCreatedBy;
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
                marketplacesString[i] = marketplaces[i].AsString();
            }

            return marketplacesString;
        }
    }
}