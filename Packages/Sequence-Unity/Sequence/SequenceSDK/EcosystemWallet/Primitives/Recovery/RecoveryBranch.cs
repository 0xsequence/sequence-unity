namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryBranch : IBranch
    {
        public ITopology[] Children { get; }
        
        public RecoveryTopology Left => Children[0] as RecoveryTopology;
        public RecoveryTopology Right => Children[1] as RecoveryTopology;

        public RecoveryBranch(RecoveryTopology left, RecoveryTopology right)
        {
            Children = new ITopology[] { left, right };
        }
    }
}