using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureOfSignerLeafErc1271 : SignatureOfSignerLeaf
    {
        public override string type => "erc1271";
        
        public Address address;
        public byte[] data;
        
        public override byte[] Encode(Leaf leaf)
        {
            if (leaf is not SignerLeaf signerLeaf)
                throw new Exception();
            
            var weightBytes = Array.Empty<byte>();
            var flag = Topology.FlagSignatureErc1271 << 4;
            var sizeLen = data.Length.MinBytesFor();
            if (sizeLen > 3)
                throw new Exception("Signature too large");

            flag |= sizeLen << 2;

            if (signerLeaf.weight <= 3 && signerLeaf.weight > 0)
                flag |= (int)signerLeaf.weight;
            else if (signerLeaf.weight <= 255)
                weightBytes = signerLeaf.weight.ByteArrayFromNumber();
            else
                throw new Exception("Weight too large");

            return flag.ByteArrayFromNumber()
                .Concat(weightBytes)
                .Concat(address.Value.HexStringToByteArray()
                    .PadLeft(20))
                .Concat(data.Length.ByteArrayFromNumber()
                    .PadLeft(sizeLen))
                .Concat(data)
                .ToArray();
        }
    }
}