using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class RawSignerLeaf : RawLeaf
    {
        public const string type = "unrecovered-signer";
        public BigInteger weight;
        public SignatureType signature;
    }

    internal abstract class SignatureType
    {
        
    }
}