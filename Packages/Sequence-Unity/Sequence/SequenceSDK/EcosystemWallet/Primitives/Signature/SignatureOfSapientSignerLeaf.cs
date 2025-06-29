using System;
using System.Linq;
using Sequence.Utils;

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

            var weightBytes = Array.Empty<byte>();
            var flag = (type == "sapient" ? Topology.FlagSignatureSapient : Topology.FlagSignatureSapientCompact) << 4;

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

            return ByteArrayExtensions.ConcatenateByteArrays(
                flag.ByteArrayFromNumber(), 
                weightBytes,
                address.Value.HexStringToByteArray()
                    .PadLeft(20),
                data.Length.ByteArrayFromNumber()
                    .PadLeft(sizeLen),
                data);
        }
    }
}