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
            
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), weightBytes, rsy.Pack());
        }
    }
}