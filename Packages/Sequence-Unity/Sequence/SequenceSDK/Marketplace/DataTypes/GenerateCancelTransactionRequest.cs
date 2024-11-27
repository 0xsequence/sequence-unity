using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GenerateCancelTransactionRequest
    {
        public string collectionAddress;
        public string maker;
        public MarketplaceKind marketplace;
        public string orderId;

        [Preserve]
        public GenerateCancelTransactionRequest(string collectionAddress, string maker, MarketplaceKind marketplace, string orderId)
        {
            this.collectionAddress = collectionAddress;
            this.maker = maker;
            this.marketplace = marketplace;
            this.orderId = orderId;
        }
    }
}