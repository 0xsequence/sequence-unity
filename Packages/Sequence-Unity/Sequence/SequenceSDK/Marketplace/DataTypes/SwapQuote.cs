using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class SwapQuote
    {
        public Address currencyAddress;
        public string currencyBalance;
        public string price;
        public string maxPrice; // Guaranteed price for the swap (including slippage)
        public Address to; // Must be approved to access maxPrice of currencyAddress for the sender
        public string transactionData;
        public string transactionValue;
        public string approveData; // Supplied when includeApprove is true

        [Preserve]
        public SwapQuote(Address currencyAddress, string currencyBalance, string price, string maxPrice, Address to, string transactionData, string transactionValue, string approveData)
        {
            this.currencyAddress = currencyAddress;
            this.currencyBalance = currencyBalance;
            this.price = price;
            this.maxPrice = maxPrice;
            this.to = to;
            this.transactionData = transactionData;
            this.transactionValue = transactionValue;
            this.approveData = approveData;
        }
    }
}