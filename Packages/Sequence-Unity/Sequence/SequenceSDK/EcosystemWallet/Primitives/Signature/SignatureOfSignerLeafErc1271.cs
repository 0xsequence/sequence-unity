namespace Sequence.EcosystemWallet.Primitives
{
    internal class SignatureOfSignerLeafErc1271 : SignatureOfSignerLeaf
    {
        public const string type = "erc1271";
        public Address address;
        public byte[] data;
    }
}