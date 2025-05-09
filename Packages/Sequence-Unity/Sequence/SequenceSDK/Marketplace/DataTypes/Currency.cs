using System;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Currency
    {
        public ulong id;
        public Chain chain;
        public string contractAddress;
        public CurrencyStatus status;
        public string name;
        public string symbol;
        public ulong decimals;
        public string imageUrl;
        public double exchangeRate;
        public bool defaultChainCurrency;
        public string createdAt;
        public string updatedAt;
        public string deletedAt;

        public const string NativeCurrencyAddress = "0x0000000000000000000000000000000000000000";
        
        public Currency(ulong id, Chain chain, string contractAddress, string name, string symbol, ulong decimals, string imageUrl, double exchangeRate, bool defaultChainCurrency, string createdAt, string updatedAt, string deletedAt = "")
        {
            this.id = id;
            this.chain = chain;
            this.contractAddress = contractAddress;
            this.name = name;
            this.symbol = symbol;
            this.decimals = decimals;
            this.imageUrl = imageUrl;
            this.exchangeRate = exchangeRate;
            this.defaultChainCurrency = defaultChainCurrency;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
            this.deletedAt = deletedAt;
            if (this.contractAddress == null)
            {
                this.contractAddress = NativeCurrencyAddress;
            }
        }

        public Currency(ulong id, Chain chain, string contractAddress, CurrencyStatus status, string name, string symbol, ulong decimals, string imageUrl, double exchangeRate, bool defaultChainCurrency, string createdAt, string updatedAt, string deletedAt = "")
        {
            this.id = id;
            this.chain = chain;
            this.contractAddress = contractAddress;
            this.status = status;
            this.name = name;
            this.symbol = symbol;
            this.decimals = decimals;
            this.imageUrl = imageUrl;
            this.exchangeRate = exchangeRate;
            this.defaultChainCurrency = defaultChainCurrency;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
            this.deletedAt = deletedAt;
            if (this.contractAddress == null)
            {
                this.contractAddress = NativeCurrencyAddress;
            }
        }

        [Preserve]
        [JsonConstructor]
        public Currency(ulong id, BigInteger chainId, string contractAddress, CurrencyStatus status, string name, string symbol, ulong decimals, string imageUrl, double exchangeRate, bool defaultChainCurrency, string createdAt, string updatedAt, string deletedAt = "")
        {
            this.id = id;
            this.chain = ChainDictionaries.ChainById[chainId.ToString()];
            this.contractAddress = contractAddress;
            this.status = status;
            this.name = name;
            this.symbol = symbol;
            this.decimals = decimals;
            this.imageUrl = imageUrl;
            this.exchangeRate = exchangeRate;
            this.defaultChainCurrency = defaultChainCurrency;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
            this.deletedAt = deletedAt;
            if (this.contractAddress == null)
            {
                this.contractAddress = NativeCurrencyAddress;
            }
        }
    }

    public static class CurrencyExtensions
    {
        public static Currency GetCurrencyByContractAddress(this Currency[] currencies, string contractAddress)
        {
            foreach (Currency currency in currencies)
            {
                if (currency.contractAddress == contractAddress)
                {
                    return currency;
                }
            }

            return null;
        }
        
        public static Currency FindDefaultChainCurrency(this Currency[] currencies)
        {
            foreach (Currency currency in currencies)
            {
                if (currency.defaultChainCurrency)
                {
                    return currency;
                }
            }

            return null;
        }
        
        public static Currency GetNativeCurrency(this Currency[] currencies)
        {
            return currencies.GetCurrencyByContractAddress(Currency.NativeCurrencyAddress);
        }

        public static bool IsActive(this Currency currency)
        {
            return currency.status == CurrencyStatus.active;
        }
    }
}