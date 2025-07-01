namespace Sequence.EcosystemWallet.Primitives
{
    public interface ILeaf
    {
        object ToJson();
        byte[] Encode();
        byte[] EncodeRaw();
    }
}