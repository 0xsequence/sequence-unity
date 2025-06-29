using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureOfSignerLeafEthSign : RSY
    {
        public override string type => "eth_sign";
        
        public override byte[] Encode(Leaf leaf)
        {
            if (leaf is not SignerLeaf signerLeaf)
                throw new Exception();
            
            var weightBytes = new byte[0];
            var flag = Topology.FlagSignatureEthSign << 4;

            if (signerLeaf.weight <= 15 && signerLeaf.weight > 0)
                flag |= (int)signerLeaf.weight;
            else if (signerLeaf.weight <= 255)
                weightBytes = signerLeaf.weight.ByteArrayFromNumber(flag.MinBytesFor());
            else
                throw new Exception("Weight too large");

            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), weightBytes, Pack());
        }
    }
}