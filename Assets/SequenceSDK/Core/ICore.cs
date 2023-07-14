using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


namespace Sequence
{
    // Sequence core primitives, inherited from go-sequence v2
    //
    // DecodeSignature takes raw signature data and returns a Signature.
    // A Signature can Recover the WalletConfig it represents.
    // A WalletConfig describes the configuration of signers that control a wallet.
    public interface ICore
    {
        // DecodeSignature takes raw signature data and returns a Signature that can Recover a WalletConfig.
        public ISignature DecodeSignature(byte[] data);

        // DecodeWalletConfig takes a decoded JSON object and returns a WalletConfig.
        public WalletConfig DecodeWalletConfig(object obj);
    }

    
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
