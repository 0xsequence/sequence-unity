using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetSwapPriceRequest
    {
        public string buyCurrencyAddress;
        public string sellCurrencyAddress;
        public string buyAmount;
        public ulong chainId;
        [FormerlySerializedAs("slippagePercentageInBasisPoints")] public ulong slippagePercentage;
        
        [Preserve]
        [JsonConstructor]
        public GetSwapPriceRequest(string buyCurrencyAddress, string sellCurrencyAddress, string buyAmount, ulong chainId, ulong slippagePercentage)
        {
            this.buyCurrencyAddress = buyCurrencyAddress;
            this.sellCurrencyAddress = sellCurrencyAddress;
            this.buyAmount = buyAmount;
            this.chainId = chainId;
            this.slippagePercentage = slippagePercentage;
        }

        public GetSwapPriceRequest(Address buyCurrencyAddress, Address sellCurrencyAddress, string buyAmount, Chain chain, ulong slippagePercentageInBasisPoints)
        {
            this.buyCurrencyAddress = buyCurrencyAddress;
            this.sellCurrencyAddress = sellCurrencyAddress;
            this.buyAmount = buyAmount;
            this.chainId = ulong.Parse(ChainDictionaries.ChainIdOf[chain]);
            this.slippagePercentage = slippagePercentageInBasisPoints;
        }
    }
}