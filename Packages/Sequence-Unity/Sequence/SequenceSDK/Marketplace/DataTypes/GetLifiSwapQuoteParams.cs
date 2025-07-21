using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetLifiSwapQuoteParams
    {
        public ulong chainId;
        public string walletAddress;
        public string fromTokenAddress;
        public string toTokenAddress;
        public string fromTokenAmount;
        public string toTokenAmount;
        public bool includeApprove;
        public ulong slippageBps;

        [Preserve]
        public GetLifiSwapQuoteParams(ulong chainId, string walletAddress, string fromTokenAddress, string toTokenAddress, bool includeApprove, ulong slippageBps, string fromTokenAmount = null, string toTokenAmount = null)
        {
            this.chainId = chainId;
            this.walletAddress = walletAddress;
            this.fromTokenAddress = fromTokenAddress;
            this.toTokenAddress = toTokenAddress;
            this.fromTokenAmount = fromTokenAmount;
            this.toTokenAmount = toTokenAmount;
            this.includeApprove = includeApprove;
            this.slippageBps = slippageBps;

            if (string.IsNullOrWhiteSpace(toTokenAmount) && string.IsNullOrWhiteSpace(fromTokenAmount))
            {
                throw new ArgumentException(
                    $"Either {nameof(fromTokenAmount)} or {nameof(toTokenAmount)} must be provided.");
            }
        }
    }
}