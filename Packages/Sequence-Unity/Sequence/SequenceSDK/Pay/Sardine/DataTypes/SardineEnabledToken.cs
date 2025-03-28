using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public class SardineEnabledToken : SardineToken
    {
        [Preserve]
        [JsonConstructor]
        public SardineEnabledToken(string network, string assetSymbol, string assetName, string chainId, string tokenName, string token, string tokenAddress)
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

        public SardineEnabledToken(Chain chain, string assetSymbol, string assetName, string tokenName, string token, Address tokenAddress)
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