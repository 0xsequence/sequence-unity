namespace Sequence.EcosystemWallet.Primitives
{
    public interface IBranch
    {
        ITopology[] Children { get; }

        byte[] Encode();
    }
}