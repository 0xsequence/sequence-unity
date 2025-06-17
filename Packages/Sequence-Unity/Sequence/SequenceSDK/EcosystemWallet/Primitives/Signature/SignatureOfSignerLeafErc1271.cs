namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureOfSignerLeafErc1271 : SignatureOfSignerLeaf
    {
        public const string type = "erc1271";
        public Address address;
        public byte[] data;
    }
}