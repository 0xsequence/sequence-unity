using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GenerateOfferTransactionArgs
    {
        public string collectionAddress;
        public string maker;
        public ContractType contractType;
        public OrderbookKind orderbookKind;
        public CreateReq offer;
        public WalletKind walletType;

        [Preserve]
        public GenerateOfferTransactionArgs(string collectionAddress, string maker, ContractType contractType, OrderbookKind orderbookKind, CreateReq offer, WalletKind walletType)
        {
            this.collectionAddress = collectionAddress;
            this.maker = maker;
            this.contractType = contractType;
            this.orderbookKind = orderbookKind;
            this.offer = offer;
            this.walletType = walletType;
        }

        public override string ToString()
        {
            return
                $"GenerateOfferTransactionArgs(collectionAddress: {collectionAddress}, maker: {maker}, contractType: {contractType}, orderbookKind: {orderbookKind}, offer: {offer}, walletType: {walletType})";
        }
    }
}