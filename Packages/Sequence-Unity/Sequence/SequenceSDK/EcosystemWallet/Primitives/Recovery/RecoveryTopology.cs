namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryTopology : ITopology
    {
        public IBranch Branch { get; }
        public ILeaf Leaf { get; }
        public INode Node { get; }

        public RecoveryTopology(RecoveryLeaf leaf)
        {
            Leaf = leaf;
        }
    }
}