using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    internal class SardineGetQuoteParams
    {
        public string assetType;
        public string network;
        public ulong total;
        public string currency;
        public SardinePaymentType paymentType;
        public SardineQuoteType quoteType;
        public Address walletAddress;
        
        private const string DefaultCurrency = "USD";

        [Preserve]
        public SardineGetQuoteParams(string assetType, string network, ulong total, Address walletAddress, string currency = DefaultCurrency, SardinePaymentType paymentType = default, SardineQuoteType quoteType = SardineQuoteType.buy)
        {
            this.assetType = assetType;
            this.network = network;
            this.total = total;
            if (string.IsNullOrWhiteSpace(currency))
            {
                currency = DefaultCurrency;
            }
            this.currency = currency;
            this.paymentType = paymentType;
            this.quoteType = quoteType;
            this.walletAddress = walletAddress;
        }
    }
}