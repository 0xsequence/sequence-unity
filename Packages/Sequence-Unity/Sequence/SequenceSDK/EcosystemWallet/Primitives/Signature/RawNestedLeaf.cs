using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class RawNestedLeaf : RawLeaf
    {
        public const string type = "nested";
        public RawTopology tree;
        public BigInteger weight;
        public BigInteger threshold;
    }
}