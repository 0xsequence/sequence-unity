namespace Sequence.EcosystemWallet.Primitives
{
    public class RawTopology
    {
        public RawLeaf Leaf { get; private set; }
        public RawNode Node { get; private set; }

        public RawTopology(RawLeaf leaf)
        {
            this.Leaf = leaf;
        }

        public RawTopology(RawNode node)
        {
            this.Node = node;
        }

        public bool IsLeaf()
        {
            return this.Leaf != null;
        }

        public bool IsNode()
        {
            return this.Node != null;
        }
    }
}