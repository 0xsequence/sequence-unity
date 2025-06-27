namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class Leaf
    {
        public abstract object Parse();
        public abstract byte[] Encode(bool noChainId, byte[] checkpointerData);
        public abstract byte[] HashConfiguration();
    }
}