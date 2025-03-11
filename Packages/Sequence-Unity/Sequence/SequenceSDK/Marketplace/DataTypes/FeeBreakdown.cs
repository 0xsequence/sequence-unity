using System;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [Serializable]
    public class FeeBreakdown
    {
        public string kind;
        public string recipientAddress;
        public BigInteger bps;

        [Preserve]
        public FeeBreakdown(string kind, string recipientAddress, BigInteger bps)
        {
            this.kind = kind;
            this.recipientAddress = recipientAddress;
            this.bps = bps;
        }
    }
}