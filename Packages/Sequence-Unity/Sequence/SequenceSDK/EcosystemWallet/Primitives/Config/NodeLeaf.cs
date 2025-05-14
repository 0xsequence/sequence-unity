namespace Sequence.EcosystemWallet.Primitives
{
    internal class NodeLeaf : Leaf
    {
        public byte[] Value;

        public static implicit operator byte[](NodeLeaf leaf)
        {
            return leaf.Value;
        }
    }
}