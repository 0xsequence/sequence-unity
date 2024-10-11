using System;

namespace Sequence.Integrations.Sardine
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

        public SardineGetQuoteParams(string assetType, string network, ulong total, Address walletAddress, string currency = "USD", SardinePaymentType paymentType = default, SardineQuoteType quoteType = default)
        {
            this.assetType = assetType;
            this.network = network;
            this.total = total;
            this.currency = currency;
            this.paymentType = paymentType;
            this.quoteType = quoteType;
            this.walletAddress = walletAddress;
        }
    }
}