namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class Leaf
    {
        public const string Signer = "signer";
        public const string Subdigest = "subdigest";
        public const string AnyAddressSubdigest = "any-address-subdigest";
        public const string Sapient = "sapient";
        public const string SapientSigner = "sapient-signer";
        public const string Nested = "nested";
        public const string UnrecoveredSigner = "unrecovered-signer";
        public const string Node = "node";
        
        public abstract object Parse();
        public abstract byte[] Encode(bool noChainId, byte[] checkpointerData);
        public abstract byte[] HashConfiguration();

        public Topology ToTopology()
        {
            return new Topology(this);
        }
    }
}