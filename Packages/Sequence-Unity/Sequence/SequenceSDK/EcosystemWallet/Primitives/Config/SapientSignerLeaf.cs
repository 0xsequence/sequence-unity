using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SapientSignerLeaf : Leaf
    {
        public Address address;
        public BigInteger weight;
        public string imageHash;
        
        public override object Parse()
        {
            return new
            {
                type = SapientSigner,
                address = address,
                weight = weight.ToString(),
                imageHash = imageHash
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            var flag = Topology.FlagNode << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), HashConfiguration());
        }

        public override byte[] HashConfiguration()
        {
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence sapient config:\n");
            byte[] address = this.address.Value.HexStringToByteArray();
            byte[] weight = this.weight.ByteArrayFromNumber(this.weight.MinBytesFor()).PadLeft(32);
            byte[] imageHash = this.imageHash.HexStringToByteArray().PadLeft(32);
                
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, address, weight, imageHash));
        }
    }
}