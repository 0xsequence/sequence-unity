using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GenerateListingTransactionArgs
    {
        public string collectionAddress;
        public string owner;
        public ContractType contractType;
        public OrderbookKind orderbookKind;
        public CreateReq listing;
        public WalletKind walletType;

        [Preserve]
        public GenerateListingTransactionArgs(string collectionAddress, string owner, ContractType contractType, OrderbookKind orderbookKind, CreateReq listing, WalletKind walletType)
        {
            this.collectionAddress = collectionAddress;
            this.owner = owner;
            this.contractType = contractType;
            this.orderbookKind = orderbookKind;
            this.listing = listing;
            this.walletType = walletType;
        }

        public override string ToString()
        {
            return
                $"GenerateListingTransactionArgs(collectionAddress: {collectionAddress}, owner: {owner}, contractType: {contractType}, orderbookKind: {orderbookKind}, listing: {listing}, walletType: {walletType})";
        }
    }
}