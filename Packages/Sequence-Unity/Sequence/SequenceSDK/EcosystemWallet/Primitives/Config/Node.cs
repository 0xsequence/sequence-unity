namespace Sequence.EcosystemWallet.Primitives
{
    public class Node
    {
        public Topology left;
        public Topology right;

        public Node(Topology left, Topology right)
        {
            this.left = left;
            this.right = right;
        }
    }
}