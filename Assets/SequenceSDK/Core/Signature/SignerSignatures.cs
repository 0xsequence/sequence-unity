using System.Collections.Generic;

namespace Sequence.Core.Signature {
    public class SignerSignatures
    {
        public Dictionary<Address, Dictionary<Hash, SignerSignature>> Data { get; set; }

        public SignerSignatures()
        {
            Data = new Dictionary<Address, Dictionary<Hash, SignerSignature>>();
        }

        public void Insert(Address signer, SignerSignature signature)
        {
            if(!Data.ContainsKey(signer))
            {
                Dictionary<Hash, SignerSignature> value = new Dictionary<Hash, SignerSignature>();
                Data[signer] = value;
            }
            Data[signer][signature.Subdigest.Hash] = signature;
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