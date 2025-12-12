using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignedSignerLeaf : SignerLeaf
    {
        private struct SignedSignerLeafJson
        {
            public string type;
            public string address;
            public string weight;
            public string signature;
        }
        
        public SignatureOfLeaf signature;

        public override object Parse()
        {
            return new SignedSignerLeafJson
            {
                type = "signed-signer",
                address = address ?? string.Empty,
                weight = weight.ToString(),
                signature = signature.Encode(this).ByteArrayToHexStringWithPrefix(),
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }
    }
}