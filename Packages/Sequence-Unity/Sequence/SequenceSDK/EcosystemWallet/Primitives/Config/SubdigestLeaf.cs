namespace Sequence.EcosystemWallet.Primitives
{
    internal class SubdigestLeaf : Leaf
    {
        public const string type = "subdigest";
        public byte[] digest;
    }
}