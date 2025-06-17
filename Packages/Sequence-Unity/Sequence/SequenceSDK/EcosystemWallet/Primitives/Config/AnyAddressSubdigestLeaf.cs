namespace Sequence.EcosystemWallet.Primitives
{
    public class AnyAddressSubdigestLeaf : Leaf
    {
        public const string type = "any-address-subdigest";
        public byte[] digest;
    }
}