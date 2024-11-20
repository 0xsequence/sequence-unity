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
        public string name;
        public string symbol;
        public ulong decimals;
        public string imageUrl;
        public double exchangeRate;
        public bool defaultChainCurrency;
        public string createdAt;
        public string updatedAt;
        public string deletedAt;

        private const string NativeCurrencyAddress = "native";

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

        [Preserve]
        [JsonConstructor]
        public Currency(ulong id, BigInteger chainId, string contractAddress, string name, string symbol, ulong decimals, string imageUrl, double exchangeRate, bool defaultChainCurrency, string createdAt, string updatedAt, string deletedAt = "")
        {
            this.id = id;
            this.chain = ChainDictionaries.ChainById[chainId.ToString()];
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
    }
}