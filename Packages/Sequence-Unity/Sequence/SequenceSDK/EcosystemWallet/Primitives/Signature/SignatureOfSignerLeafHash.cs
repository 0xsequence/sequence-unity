using System;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureOfSignerLeafHash : SignatureOfSignerLeaf
    {
        public override string type => "hash";

        public RSY rsy;
        
        public override byte[] Encode(Leaf leaf)
        {
            if (leaf is not SignerLeaf signerLeaf)
                throw new Exception($"Leaf type is not supported: {leaf.GetType()}");

            var weight = signerLeaf.weight;
            var weightBytes = Array.Empty<byte>();
            
            var flag = Topology.FlagSignatureHash << 4;

            if (weight <= 15 && weight > 0)
                flag |= (int)weight;
            else if (weight <= 255)
                weightBytes = weight.ByteArrayFromNumber(weight.MinBytesFor());
            else
                throw new Exception("Weight too large");
            
            if ("0x020213a31d1a2ec622361e3285cc7377bb05ad8a7ee7db48551f9d94e5036a1306de1d615daff8918cd9180a9ac57c6dd590beb8d567b4ad8ecc4ca7b05296895916".Contains(ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), weightBytes, rsy.Pack()).ByteArrayToHexString()))
                Debug.Log($"##1 {ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), weightBytes, rsy.Pack()).ByteArrayToHexString()}: {weight}, {rsy.r}, {rsy.s}, {rsy.yParity}");

            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), weightBytes, rsy.Pack());
        }
    }
}