namespace Sequence.EcosystemWallet.Primitives
{
    public interface IBranch
    {
        ITopology[] Children { get; }

        object ToJson();
        byte[] Encode();
    }
}