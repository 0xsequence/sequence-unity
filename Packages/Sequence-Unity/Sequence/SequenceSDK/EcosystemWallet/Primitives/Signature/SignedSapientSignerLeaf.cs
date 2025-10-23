using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignedSapientSignerLeaf : SapientSignerLeaf
    {
        private struct SignedSapientSignerLeafJson
        {
            public string type;
            public string address;
            public string weight;
            public string imageHash;
            public string signature;
        }
        
        public SignatureOfLeaf signature;

        public override object Parse()
        {
            return new SignedSapientSignerLeafJson
            {
                type = "signed-sapient-signer",
                address = address,
                weight = weight.ToString(),
                imageHash = imageHash,
                signature = signature.Encode(this).ByteArrayToHexStringWithPrefix()
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }
    }
}