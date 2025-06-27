using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Envelope
{
    public class Signature : EnvelopeSignature
    {
        public Address address;
        public SignatureOfSignerLeaf signature;
        public override string type { get; }
        public override byte[] Encode(Leaf leaf)
        {
            throw new System.NotImplementedException();
        }
    }
}