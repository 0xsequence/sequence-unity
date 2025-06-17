using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SapientSignerLeaf : Leaf
    {
        public const string type = "sapient-signer";
        public Address address;
        public BigInteger weight;
        public string imageHash;
        public bool signed;
        public SignatureType signature;
    }
}