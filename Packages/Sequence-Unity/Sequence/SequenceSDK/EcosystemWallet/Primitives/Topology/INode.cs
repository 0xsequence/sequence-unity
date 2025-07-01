namespace Sequence.EcosystemWallet.Primitives
{
    public interface INode
    {
        byte[] Value { get; }

        object ToJsonObject();
        byte[] Encode();
    }
}