using System;
using Newtonsoft.Json;
using Sequence.EmbeddedWallet;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetSwapQuoteRequest
    {
        public string userAddress;
        public string buyCurrencyAddress;
        public string sellCurrencyAddress;
        public string buyAmount;
        public ulong chainId;
        public bool includeApprove;
        public ulong slippagePercentage;

        [Preserve]
        [JsonConstructor]
        public GetSwapQuoteRequest(string userAddress, string buyCurrencyAddress, string sellCurrencyAddress,
            string buyAmount, ulong chainId, bool includeApprove, ulong slippagePercentage)
        {
            this.userAddress = userAddress;
            this.buyCurrencyAddress = buyCurrencyAddress;
            this.sellCurrencyAddress = sellCurrencyAddress;
            this.buyAmount = buyAmount;
            this.chainId = chainId;
            this.slippagePercentage = slippagePercentage;
            this.includeApprove = includeApprove;
        }

        public GetSwapQuoteRequest(Address userAddress, Address buyCurrencyAddress, Address sellCurrencyAddress,
            string buyAmount, Chain chain, ulong slippagePercentage, bool includeApprove)
        {
            this.userAddress = userAddress;
            this.buyCurrencyAddress = buyCurrencyAddress;
            this.sellCurrencyAddress = sellCurrencyAddress;
            this.buyAmount = buyAmount;
            this.chainId = ulong.Parse(ChainDictionaries.ChainIdOf[chain]);
            this.slippagePercentage = slippagePercentage;
            this.includeApprove = includeApprove;
        }
    }
}