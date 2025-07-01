namespace Sequence.EcosystemWallet.Primitives
{
    public interface ITopology
    {
        IBranch Branch { get; }
        ILeaf Leaf { get; }
        INode Node { get; }
    }
}