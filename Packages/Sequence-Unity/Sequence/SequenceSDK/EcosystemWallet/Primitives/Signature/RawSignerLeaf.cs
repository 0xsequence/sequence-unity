using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RawSignerLeaf : Leaf
    {
        public const string type = "unrecovered-signer";
        
        public BigInteger weight;
        public SignatureType signature;
        
        public override object Parse()
        {
            throw new System.NotImplementedException("RawSignerLeaf.Parse");
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }

        public override byte[] HashConfiguration()
        {
            throw new System.NotImplementedException("RawSignerLeaf.HashConfiguration");
        }
    }

    public abstract class SignatureType
    {
        public abstract string type { get; }

        public abstract byte[] Encode(Leaf leaf);
    }
}