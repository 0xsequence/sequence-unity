using System;
using System.Collections.Generic;
using Sequence.EmbeddedWallet;
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
        public string amount;
        public string amountMin;

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

        public SwapQuote(LifiSwapQuote quote)
        {
            this.currencyAddress = quote.currencyAddress;
            this.currencyBalance = quote.currencyBalance;
            this.price = quote.price;
            this.maxPrice = quote.maxPrice;
            this.to = quote.to;
            this.transactionData = quote.transactionData;
            this.transactionValue = quote.transactionValue;
            this.approveData = quote.approveData;
            this.amount = quote.amount;
            this.amountMin = quote.amountMin;
        }
    }
    
    public static class SwapQuoteExtensions
    {
        public static Transaction[] AsTransactionArray(this SwapQuote swapQuote)
        {
            List<Transaction> transactions = new List<Transaction>();
            if (!string.IsNullOrWhiteSpace(swapQuote.approveData))
            {
                transactions.Add(new RawTransaction(swapQuote.to, null, swapQuote.approveData));
            }
            transactions.Add(new RawTransaction(swapQuote.to, swapQuote.transactionValue, swapQuote.transactionData));
            
            return transactions.ToArray();
        }
    }
}