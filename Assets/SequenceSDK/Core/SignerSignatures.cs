using System.Collections.Generic;

namespace Sequence.Core {
    public class SignerSignatures
    {
        //From go-sequence :
        //type SignerSignatures map[common.Address]map[common.Hash]SignerSignature
        //TODO: address type and hash type
        public Dictionary<string, Dictionary<string, SignerSignature>> Data { get; set; }

        public SignerSignatures()
        {
            Data = new Dictionary<string, Dictionary<string, SignerSignature>>();
        }

        public void Insert(string signerAddress, SignerSignature signature)
        {
            if(!Data.ContainsKey(signerAddress))
            {
                Dictionary<string, SignerSignature> value = new Dictionary<string, SignerSignature>();
                value.Add(signature.Subdigest.Hash, signature);
                Data.Add(signerAddress, value);
            }

        }
    }

    public class SignerSignature
    {
        public Subdigest Subdigest { get; set; }
        public SignerSignatureType Type { get; set; }
        public byte[] Signature { get; set; }

    }

    public enum SignerSignatureType
    {
        SignerSignatureTypeEIP712,
        SignerSignatureTypeEthSign,
        SignerSignatureTypeEIP1271
    }
}