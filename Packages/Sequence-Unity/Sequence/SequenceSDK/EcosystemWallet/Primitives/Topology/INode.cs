namespace Sequence.EcosystemWallet.Primitives
{
    public interface INode
    {
        byte[] Value { get; }

        object ToJson();
        byte[] Encode();
    }
}