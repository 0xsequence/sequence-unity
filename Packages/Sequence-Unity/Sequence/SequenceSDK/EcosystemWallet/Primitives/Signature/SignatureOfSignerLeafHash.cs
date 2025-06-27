using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureOfSignerLeafHash : RSY
    {
        public override string type => "hash";
        
        public override byte[] Encode(Leaf leaf)
        {
            if (leaf is not SignerLeaf signerLeaf)
                throw new Exception();
            
            var weightBytes = Array.Empty<byte>();
            var flag = Topology.FlagSignatureHash << 4;

            if (signerLeaf.weight <= 15 && signerLeaf.weight > 0)
                flag |= (int)signerLeaf.weight;
            else if (signerLeaf.weight <= 255)
                weightBytes = signerLeaf.weight.ByteArrayFromNumber();
            else
                throw new Exception("Weight too large");

            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(), weightBytes, Pack());
        }
    }
}