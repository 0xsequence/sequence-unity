using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetSwapPricesRequest
    {
        public string userAddress;
        public string buyCurrencyAddress;
        public string buyAmount;
        public ulong chainId;
        public ulong slippagePercentage;

        [Preserve]
        [JsonConstructor]
        public GetSwapPricesRequest(string userAddress, string buyCurrencyAddress, string buyAmount, ulong chainId, ulong slippagePercentage)
        {
            this.userAddress = userAddress;
            this.buyCurrencyAddress = buyCurrencyAddress;
            this.buyAmount = buyAmount;
            this.chainId = chainId;
            this.slippagePercentage = slippagePercentage;
        }
        
        public GetSwapPricesRequest(Address userAddress, Address buyCurrencyAddress, string buyAmount, Chain chain, ulong slippagePercentage)
        {
            this.userAddress = userAddress;
            this.buyCurrencyAddress = buyCurrencyAddress;
            this.buyAmount = buyAmount;
            this.chainId = ulong.Parse(ChainDictionaries.ChainIdOf[chain]);
            this.slippagePercentage = slippagePercentage;
        }
    }
}