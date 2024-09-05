using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GenerateBuyTransaction
    {
        public string collectionAddress;
        public string buyer;
        public MarketplaceKind marketplace;
        public OrderData[] ordersData;
        public AdditionalFee[] additionalFees;
        public WalletKind walletType;

        public GenerateBuyTransaction(string collectionAddress, string buyer, MarketplaceKind marketplace, OrderData[] ordersData, AdditionalFee[] additionalFees, WalletKind walletType)
        {
            this.collectionAddress = collectionAddress;
            this.buyer = buyer;
            this.marketplace = marketplace;
            this.ordersData = ordersData;
            this.additionalFees = additionalFees;
            this.walletType = walletType;
        }
    }
}