using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Envelope
{
    public class Signature : EnvelopeSignature
    {
        public override string type { get; }
        
        public Address address;
        public SignatureOfSignerLeaf signature;
        
        public override byte[] Encode(Leaf leaf)
        {
            return signature.Encode(leaf);
        }
    }
}