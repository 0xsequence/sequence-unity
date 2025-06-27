using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class RawNestedLeaf : Leaf
    {
        public const string type = "nested";
        public Topology tree;
        public BigInteger weight;
        public BigInteger threshold;
        
        public override object Parse()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] HashConfiguration()
        {
            throw new System.NotImplementedException();
        }
    }
}