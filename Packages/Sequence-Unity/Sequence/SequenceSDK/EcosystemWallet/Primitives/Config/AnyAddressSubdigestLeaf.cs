namespace Sequence.EcosystemWallet.Primitives
{
    internal class AnyAddressSubdigestLeaf : Leaf
    {
        public const string type = "any-address-subdigest";
        public byte[] digest;
    }
}