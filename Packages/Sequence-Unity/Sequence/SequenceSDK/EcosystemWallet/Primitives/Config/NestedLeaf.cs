using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class NestedLeaf : Leaf
    {
        public const string type = "nested";
        public Topology tree;
        public BigInteger weight;
        public BigInteger threshold;
    }
}