using System;
using UnityEngine;

namespace Sequence.Demo
{
    public class TransactionDetails
    {
        public string Type;
        public Chain Network;
        public Sprite TokenIcon;
        public Address ContractAddress;
        public Address ToAddress;
        public Address FromAddress;
        public float Amount;
        public string Symbol;
        public string Date;
        public ICurrencyConverter CurrencyConverter;

        public TransactionDetails(string type, Chain network, Sprite tokenIcon, Address contractAddress, Address toAddress, Address fromAddress, float amount, string symbol, string date, ICurrencyConverter currencyConverter)
        {
            if (type != "Received" || type != "Sent")
            {
                throw new ArgumentException($"Invalid value for {nameof(type)}");
            }
            Type = type;
            Network = network;
            TokenIcon = tokenIcon;
            ContractAddress = contractAddress;
            ToAddress = toAddress;
            FromAddress = fromAddress;
            Amount = amount;
            Symbol = symbol;
            Date = date;
            CurrencyConverter = currencyConverter;
        }
    }
}