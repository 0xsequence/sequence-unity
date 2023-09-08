using System;
using UnityEngine;

namespace Sequence.Demo
{
    public class TransactionDetails : ICurrencyRepository
    {
        public readonly string Type;
        public readonly Chain Network;
        public readonly Sprite TokenIcon;
        public readonly Address ContractAddress;
        public Address ToAddress;
        public Address FromAddress;
        private readonly uint _amount;
        public readonly string Symbol;
        public readonly string Date;
        private readonly ICurrencyConverter _currencyConverter;

        public TransactionDetails(string type, Chain network, Sprite tokenIcon, Address contractAddress, Address toAddress, Address fromAddress, uint amount, string symbol, string date, ICurrencyConverter currencyConverter)
        {
            if (type != "Received" && type != "Sent")
            {
                throw new ArgumentException($"Invalid value for {nameof(type)}");
            }
            Type = type;
            Network = network;
            TokenIcon = tokenIcon;
            ContractAddress = contractAddress;
            ToAddress = toAddress;
            FromAddress = fromAddress;
            _amount = amount;
            Symbol = symbol;
            Date = date;
            _currencyConverter = currencyConverter;
        }

        public uint GetAmount()
        {
            return _amount;
        }

        public void SetAmount(uint value)
        {
            throw new NotSupportedException($"A {nameof(TransactionDetails)} cannot have its amount modified after creation");
        }

        public string GetAddress()
        {
            return ContractAddress;
        }

        public string GetSymbol()
        {
            return Symbol;
        }

        public float GetPreviousCurrencyValue()
        {
            throw new NotSupportedException($"A {nameof(TransactionDetailsBlock)} does not need to know anything about changes in currency value so we do not keep track of previous currency values.");
        }

        public void SetPreviousCurrencyValue(float value)
        {
            // Do nothing
        }

        public Currency GetCurrency()
        {
            return _currencyConverter.ConvertToCurrency(GetAmount(), GetAddress());
        }
    }
}