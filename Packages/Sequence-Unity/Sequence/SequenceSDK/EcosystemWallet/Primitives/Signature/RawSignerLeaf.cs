using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RawSignerLeaf : RawLeaf
    {
        public const string type = "unrecovered-signer";
        public BigInteger weight;
        public SignatureType signature;
    }

    public abstract class SignatureType
    {
        
    }
}