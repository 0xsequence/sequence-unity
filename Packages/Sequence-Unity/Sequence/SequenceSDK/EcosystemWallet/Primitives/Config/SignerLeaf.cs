using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignerLeaf : Leaf
    {
        public const string type = "signer";
        public Address address;
        public BigInteger weight;
        public bool signed;
        public SignatureType signature;
    }
}