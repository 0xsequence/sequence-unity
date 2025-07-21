using System;
using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetLifiSwapRoutesArgs
    {
        public BigInteger chainId;
        public string toTokenAddress;
        public string toTokenAmount;
        public string walletAddress;

        [Preserve]
        public GetLifiSwapRoutesArgs(BigInteger chainId, string toTokenAddress, string toTokenAmount, string walletAddress)
        {
            this.chainId = chainId;
            this.toTokenAddress = toTokenAddress;
            this.toTokenAmount = toTokenAmount;
            this.walletAddress = walletAddress;
        }
    }
}