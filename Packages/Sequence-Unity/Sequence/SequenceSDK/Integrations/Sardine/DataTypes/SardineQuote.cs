using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineQuote
    {
        public double quantity;
        public string network;
        public string assetType;
        public double total;
        public string currency;
        public string expiresAt;
        public string paymentType;
        public double price;
        public double subtotal;
        public double transactionFee;
        public double networkFee;
        public bool highNetworkFee;
        public double minTransactionValue;
        public double maxTransactionValue;
        public string liquidityProvider;

        [Preserve]
        public SardineQuote(double quantity, string network, string assetType, double total, string currency, string expiresAt, string paymentType, double price, double subtotal, double transactionFee, double networkFee, bool highNetworkFee, double minTransactionValue, double maxTransactionValue, string liquidityProvider)
        {
            this.quantity = quantity;
            this.network = network;
            this.assetType = assetType;
            this.total = total;
            this.currency = currency;
            this.expiresAt = expiresAt;
            this.paymentType = paymentType;
            this.price = price;
            this.subtotal = subtotal;
            this.transactionFee = transactionFee;
            this.networkFee = networkFee;
            this.highNetworkFee = highNetworkFee;
            this.minTransactionValue = minTransactionValue;
            this.maxTransactionValue = maxTransactionValue;
            this.liquidityProvider = liquidityProvider;
        }
    }
}