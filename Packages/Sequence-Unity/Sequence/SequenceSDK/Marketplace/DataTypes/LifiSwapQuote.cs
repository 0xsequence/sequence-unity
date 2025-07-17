using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    public class LifiSwapQuote
    {
        public Address currencyAddress;
        public string currencyBalance;
        public string price;
        public string maxPrice; // Guaranteed price for the swap (including slippage)
        public Address to;
        public string transactionData;
        public string transactionValue;
        public string approveData; // Included only if includeApprove is true in request parameters
        public string amount;
        public string amountMin;
        
        public LifiSwapQuote(Address currencyAddress, string currencyBalance, string price, string maxPrice, Address to, string transactionData, string transactionValue, string approveData, string amount, string amountMin)
        {
            this.currencyAddress = currencyAddress;
            this.currencyBalance = currencyBalance;
            this.price = price;
            this.maxPrice = maxPrice;
            this.to = to;
            this.transactionData = transactionData;
            this.transactionValue = transactionValue;
            this.approveData = approveData;
            this.amount = amount;
            this.amountMin = amountMin;
        }
        
        [JsonConstructor]
        [Preserve]
        public LifiSwapQuote(string currencyAddress, string currencyBalance, string price, string maxPrice, string to, string transactionData, string transactionValue, string approveData, string amount, string amountMin)
        {
            this.currencyAddress = new Address(currencyAddress);
            this.currencyBalance = currencyBalance;
            this.price = price;
            this.maxPrice = maxPrice;
            this.to = new Address(to);
            this.transactionData = transactionData;
            this.transactionValue = transactionValue;
            this.approveData = approveData;
            this.amount = amount;
            this.amountMin = amountMin;
        }
    }
}