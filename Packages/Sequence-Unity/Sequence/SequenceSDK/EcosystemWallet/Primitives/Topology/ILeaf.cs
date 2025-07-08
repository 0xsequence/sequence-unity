namespace Sequence.EcosystemWallet.Primitives
{
    public interface ILeaf
    {
        object ToJsonObject();
        byte[] Encode();
        byte[] EncodeRaw();
    }
}