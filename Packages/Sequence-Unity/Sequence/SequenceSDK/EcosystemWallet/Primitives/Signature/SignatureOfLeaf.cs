namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class SignatureOfLeaf
    {
        public abstract string type { get; }

        public abstract byte[] Encode(Leaf leaf);
    }
}