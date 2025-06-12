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
        
        public Leaf FindSignerLeaf(Address address)
        {
            if (IsNode())
            {
                Leaf leftResult = Node.left.FindSignerLeaf(address);
                if (leftResult != null)
                {
                    return leftResult;
                }
                return Node.right.FindSignerLeaf(address);
            }
            else if (IsLeaf())
            {
                if (Leaf.isSignerLeaf)
                {
                    SignerLeaf signerLeaf = (SignerLeaf)Leaf;
                    if (signerLeaf.address.Equals(address))
                    {
                        return signerLeaf;
                    }
                }
                else if (Leaf.isSapientSignerLeaf)
                {
                    SapientSignerLeaf sapientLeaf = (SapientSignerLeaf)Leaf;
                    if (sapientLeaf.address.Equals(address))
                    {
                        return sapientLeaf;
                    }
                }
            }
            return null;
        }
    }
}