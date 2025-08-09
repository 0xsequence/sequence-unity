using System;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignerLeaf : Leaf
    {
        public Address address;
        public BigInteger weight;
        
        public override object Parse()
        {
            return new
            {
                type = Signer,
                address = address,
                weight = weight.ToString()
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            var flag = Topology.FlagAddress << 4;
            var weightBytes = Array.Empty<byte>();

            if (weight > 0 && weight <= 15)
                flag |= (int)weight;
            else if (weight <= 255)
                weightBytes = weight.ByteArrayFromNumber(flag.MinBytesFor());
            else
                throw new ArgumentException("Weight too large");
            
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), weightBytes,
                address.Value.HexStringToByteArray().PadLeft(20));
        }

        public override byte[] HashConfiguration()
        {
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence signer:\n");
            byte[] address = this.address.Value.HexStringToByteArray();
            byte[] weight = this.weight.ByteArrayFromNumber(this.weight.MinBytesFor()).PadLeft(32);
                
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, address, weight));
        }
    }
}