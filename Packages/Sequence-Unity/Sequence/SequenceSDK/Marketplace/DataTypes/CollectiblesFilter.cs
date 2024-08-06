using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CollectiblesFilter
    {
        public bool includeEmpty;
        public string searchText = "";
        public PropertyFilter[] properties = null;
        public MarketplaceKind[] marketplaces = null;
        public string[] inAccounts = null;
        public string[] notInAccounts = null;
        public string[] ordersCreatedBy = null;
        public string[] ordersNotCreatedBy = null;
        
        public CollectiblesFilter(bool includeEmpty, string searchText = "", PropertyFilter[] properties = null, MarketplaceKind[] marketplaces = null, string[] inAccounts = null, string[] notInAccounts = null, string[] ordersCreatedBy = null, string[] ordersNotCreatedBy = null)
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
    }
}