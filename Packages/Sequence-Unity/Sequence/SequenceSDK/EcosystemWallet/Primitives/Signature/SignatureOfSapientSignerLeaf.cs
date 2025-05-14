namespace Sequence.EcosystemWallet.Primitives
{
    internal class SignatureOfSapientSignerLeaf : SignatureType
    {
        public Address address;
        public byte[] data;
        public Type type;
        
        public enum Type
        {
            sapient,
            sapient_compact
        }
    }
}