using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GenerateListingTransactionArgs
    {
        public string collectionAddress;
        public string owner;
        public ContractType contractType;
        public OrderbookKind orderbook;
        public CreateReq listing;
        public WalletKind walletType;

        [Preserve]
        public GenerateListingTransactionArgs(string collectionAddress, string owner, ContractType contractType, OrderbookKind orderbook, CreateReq listing, WalletKind walletType)
        {
            this.collectionAddress = collectionAddress;
            this.owner = owner;
            this.contractType = contractType;
            this.orderbook = orderbook;
            this.listing = listing;
            this.walletType = walletType;
        }

        public override string ToString()
        {
            return
                $"GenerateListingTransactionArgs(collectionAddress: {collectionAddress}, owner: {owner}, contractType: {contractType}, orderbook: {orderbook}, listing: {listing}, walletType: {walletType})";
        }
    }
}