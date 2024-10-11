using System;
using Newtonsoft.Json;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SardineSupportedToken
    {
        public string network;
        public string assetSymbol;
        public string assetName;
        public string chainId;
        public string tokenName;
        public string token;
        public Address tokenAddress;

        [JsonConstructor]
        public SardineSupportedToken(string network, string assetSymbol, string assetName, string chainId, string tokenName, string token, Address tokenAddress)
        {
            this.network = network;
            this.assetSymbol = assetSymbol;
            this.assetName = assetName;
            this.chainId = chainId;
            this.tokenName = tokenName;
            this.token = token;
            this.tokenAddress = tokenAddress;
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
    }
}