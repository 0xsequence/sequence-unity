using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Envelope
{
    public class SapientSignature : EnvelopeSignature
    {
        public string imageHash;
        public SignatureOfSapientSignerLeaf signature;
        public override string type { get; }
        
        public override byte[] Encode(Leaf leaf)
        {
            return signature.Encode(leaf);
        }

        public string ToString()
        {
            return $"Envelope SapientSignature, Image Hash: {imageHash}, Address: {signature.address}, Type: {signature.type}, Data: {signature.data.ByteArrayToHexStringWithPrefix()}";
        }
    }
}