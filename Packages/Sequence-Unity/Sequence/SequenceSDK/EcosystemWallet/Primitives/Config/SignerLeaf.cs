using System;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignerLeaf : Leaf
    {
        public const string type = "signer";
        
        public Address address;
        public BigInteger weight;
        
        public override object Parse()
        {
            return new
            {
                type = type,
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
                weightBytes = weight.ByteArrayFromNumber();
            else
                throw new ArgumentException("Weight too large");

            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(), weightBytes,
                address.Value.HexStringToByteArray().PadLeft(20));
        }

        public override byte[] HashConfiguration()
        {
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence signer:\n");
            byte[] address = this.address.Value.HexStringToByteArray();
            byte[] weight = this.weight.ByteArrayFromNumber().PadLeft(32);
                
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, address, weight));
        }
    }
}