using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GenerateSellTransaction
    {
        public string collectionAddress;
        public string seller;
        public MarketplaceKind marketplace;
        public OrderData[] ordersData;
        public AdditionalFee[] additionalFees;
        public WalletKind walletType;

        [Preserve]
        public GenerateSellTransaction(string collectionAddress, string seller, MarketplaceKind marketplace, OrderData[] ordersData, AdditionalFee[] additionalFees, WalletKind walletType)
        {
            this.collectionAddress = collectionAddress;
            this.seller = seller;
            this.marketplace = marketplace;
            this.ordersData = ordersData;
            this.additionalFees = additionalFees;
            this.walletType = walletType;
        }
    }
}