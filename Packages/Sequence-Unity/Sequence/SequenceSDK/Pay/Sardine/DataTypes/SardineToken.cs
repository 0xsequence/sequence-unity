using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public abstract class SardineToken 
    {
        public string network;
        public string assetSymbol;
        public string assetName;
        public string chainId;
        public string tokenName;
        public string token;
        public Address tokenAddress;
        
        public bool MatchesChain(Chain chain)
        {
            if (string.IsNullOrWhiteSpace(chainId))
            {
                return false;
            }
            return ChainDictionaries.ChainById.ContainsKey(chainId) && ChainDictionaries.ChainById[chainId] == chain;
        }
    }
}