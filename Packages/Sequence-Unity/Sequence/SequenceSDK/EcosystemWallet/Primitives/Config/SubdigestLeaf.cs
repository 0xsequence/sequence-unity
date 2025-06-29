using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SubdigestLeaf : Leaf
    {
        public const string type = "subdigest";
        public byte[] digest;
        
        public override object Parse()
        {
            return new
            {
                type = "subdigest",
                digest = digest.ByteArrayToHexString()
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            var flag = Topology.FlagSubdigest << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), digest);
        }

        public override byte[] HashConfiguration()
        {
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence static digest:\n");
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, digest));
        }
    }
}