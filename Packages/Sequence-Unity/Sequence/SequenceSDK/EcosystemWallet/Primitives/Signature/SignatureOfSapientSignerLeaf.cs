using System;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureOfSapientSignerLeaf : SignatureOfLeaf
    {
        public enum Type
        {
            sapient,
            sapient_compact
        }
        
        public override string type => curType.ToString();
        
        public Address address;
        public byte[] data;
        public Type curType;
        
        public override byte[] Encode(Leaf leaf)
        {
            if (leaf is not SapientSignerLeaf signerLeaf)
                throw new Exception();

            var weight = signerLeaf.weight;
            var weightBytes = Array.Empty<byte>();
            
            var flag = (type == "sapient" ? 
                Topology.FlagSignatureSapient : 
                Topology.FlagSignatureSapientCompact) 
                       << 4;

            var bytesForSignatureSize = data.Length.MinBytesFor();
            if (bytesForSignatureSize > 3)
                throw new Exception("Signature too large");

            flag |= bytesForSignatureSize << 2;

            if (weight <= 3 && weight > 0)
                flag |= (int)weight;
            else if (weight <= 255)
                weightBytes = weight.ByteArrayFromNumber(weight.MinBytesFor());
            else
                throw new Exception("Weight too large");
            
            Debug.Log($"SignatureOfSapientSignerLeaf flag {flag.ByteArrayFromNumber(flag.MinBytesFor()).ByteArrayToHexString()}");
            Debug.Log($"SignatureOfSapientSignerLeaf weight {weightBytes.ByteArrayToHexString()}");
            Debug.Log($"SignatureOfSapientSignerLeaf address {address.Value.HexStringToByteArray(20).ByteArrayToHexString()}");
            Debug.Log($"SignatureOfSapientSignerLeaf dataLength {data.Length.ByteArrayFromNumber(bytesForSignatureSize).ByteArrayToHexString()}");
            Debug.Log($"SignatureOfSapientSignerLeaf data {data.ByteArrayToHexString()}");

            return ByteArrayExtensions.ConcatenateByteArrays(
                flag.ByteArrayFromNumber(flag.MinBytesFor()), 
                weightBytes,
                address.Value.HexStringToByteArray(20),
                data.Length.ByteArrayFromNumber(bytesForSignatureSize),
                data);
        }
    }
}