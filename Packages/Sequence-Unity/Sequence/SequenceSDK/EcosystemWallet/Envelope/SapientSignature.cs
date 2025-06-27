using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Envelope
{
    public class SapientSignature : EnvelopeSignature
    {
        public string imageHash;
        public SignatureOfSapientSignerLeaf signature;
        public override string type { get; }
        public override byte[] Encode(Leaf leaf)
        {
            throw new System.NotImplementedException();
        }
    }
}