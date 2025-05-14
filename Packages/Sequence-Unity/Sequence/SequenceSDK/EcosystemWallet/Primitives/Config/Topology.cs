namespace Sequence.EcosystemWallet.Primitives
{
    internal class Topology
    {
        public Node Node;
        public Leaf Leaf;

        public Topology(Leaf leaf)
        {
            this.Leaf = leaf;
        }

        public Topology(Node node)
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