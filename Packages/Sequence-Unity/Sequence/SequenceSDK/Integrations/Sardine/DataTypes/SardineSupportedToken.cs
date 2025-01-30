using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineSupportedToken : IChainMatcher
    {
        public string network;
        public string assetSymbol;
        public string assetName;
        public string chainId;
        public string tokenName;
        public string token;
        public Address tokenAddress;

        [Preserve]
        [JsonConstructor]
        public SardineSupportedToken(string network, string assetSymbol, string assetName, string chainId, string tokenName, string token, string tokenAddress)
        {
            this.network = network;
            this.assetSymbol = assetSymbol;
            this.assetName = assetName;
            this.chainId = chainId;
            this.tokenName = tokenName;
            this.token = token;
            if (network != "stellar" && !string.IsNullOrWhiteSpace(tokenAddress))
            {
                this.tokenAddress = new Address(tokenAddress);
            }
            else
            {
                this.tokenAddress = null;
            }
        }

        public SardineSupportedToken(Chain chain, string assetSymbol, string assetName, string tokenName, string token, Address tokenAddress)
        {
            this.network = ChainDictionaries.NameOf[chain];
            this.assetSymbol = assetSymbol;
            this.assetName = assetName;
            this.chainId = ChainDictionaries.ChainIdOf[chain];
            this.tokenName = tokenName;
            this.token = token;
            this.tokenAddress = tokenAddress;
        }

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