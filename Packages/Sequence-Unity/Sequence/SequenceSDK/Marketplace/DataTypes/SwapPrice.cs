using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class SwapPrice
    {
        public Address currencyAddress;
        [Obsolete("currencyBalance is no longer supported by swap provider")]
        public string currencyBalance;
        public string price;
        [Obsolete("To retrieve the max price, please request a Swap Quote instead")]
        public string maxPrice; // Guaranteed price for the swap (including slippage)
        [Obsolete("To retrieve the transaction value, please request a Swap Quote instead")]
        public string transactionValue;
        
        public SwapPrice(Address currencyAddress, string currencyBalance, string price, string maxPrice, string transactionValue)
        {
            this.currencyAddress = currencyAddress;
            this.currencyBalance = currencyBalance;
            this.price = price;
            this.maxPrice = maxPrice;
            this.transactionValue = transactionValue;
        }

        public SwapPrice(Address currencyAddress, string price)
        {
            this.currencyAddress = currencyAddress;
            this.price = price;
        }
    }
}