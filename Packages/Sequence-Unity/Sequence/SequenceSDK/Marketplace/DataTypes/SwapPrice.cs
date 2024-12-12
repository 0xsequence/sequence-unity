using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class SwapPrice
    {
        public Address currencyAddress;
        public string currencyBalance;
        public string price;
        public string maxPrice; // Guaranteed price for the swap (including slippage)
        public string transactionValue;

        [Preserve]
        public SwapPrice(Address currencyAddress, string currencyBalance, string price, string maxPrice, string transactionValue)
        {
            this.currencyAddress = currencyAddress;
            this.currencyBalance = currencyBalance;
            this.price = price;
            this.maxPrice = maxPrice;
            this.transactionValue = transactionValue;
        }
    }
}