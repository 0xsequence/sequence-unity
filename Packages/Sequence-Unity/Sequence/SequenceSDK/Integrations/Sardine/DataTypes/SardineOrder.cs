using System;
using Newtonsoft.Json;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineOrder
    {
        public string id;
        public string createdAt;
        public string referenceId;
        public string status;
        public string fiatCurrency;
        public double fiatExchangeRateUSD;
        public string transactionId;
        public string expiresAt;
        public double total;
        public double subTotal;
        public double transactionFee;
        public double networkFee;
        public string paymentCurrency;
        public string paymentMethodType;
        public string transactionType;
        public string name;
        public int price;
        public string imageUrl;
        public Address contractAddress;
        public string transactionHash;
        public Address recipientAddress;

        public SardineOrder(string id, string createdAt, string referenceId, string status, string fiatCurrency, double fiatExchangeRateUsd, string transactionId, string expiresAt, double total, double subTotal, double transactionFee, double networkFee, string paymentCurrency, string paymentMethodType, string transactionType, string name, int price, string imageUrl, Address contractAddress = null, string transactionHash = null, Address recipientAddress = null)
        {
            this.id = id;
            this.createdAt = createdAt;
            this.referenceId = referenceId;
            this.status = status;
            this.fiatCurrency = fiatCurrency;
            fiatExchangeRateUSD = fiatExchangeRateUsd;
            this.transactionId = transactionId;
            this.expiresAt = expiresAt;
            this.total = total;
            this.subTotal = subTotal;
            this.transactionFee = transactionFee;
            this.networkFee = networkFee;
            this.paymentCurrency = paymentCurrency;
            this.paymentMethodType = paymentMethodType;
            this.transactionType = transactionType;
            this.name = name;
            this.price = price;
            this.imageUrl = imageUrl;
            this.contractAddress = contractAddress;
            this.transactionHash = transactionHash;
            this.recipientAddress = recipientAddress;
        }
    }
}