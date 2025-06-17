namespace Sequence.EcosystemWallet.Primitives
{
    public class SubdigestLeaf : Leaf
    {
        public const string type = "subdigest";
        public byte[] digest;
    }
}