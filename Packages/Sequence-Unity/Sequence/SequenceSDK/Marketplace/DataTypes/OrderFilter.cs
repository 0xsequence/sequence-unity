using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class OrderFilter
    {
        public string[] createdBy;
        public MarketplaceKind[] marketplace;
        public string[] currencies;

        public OrderFilter(string[] createdBy, MarketplaceKind[] marketplace, string[] currencies)
        {
            this.createdBy = createdBy;
            this.marketplace = marketplace;
            this.currencies = currencies;
        }
    }
}