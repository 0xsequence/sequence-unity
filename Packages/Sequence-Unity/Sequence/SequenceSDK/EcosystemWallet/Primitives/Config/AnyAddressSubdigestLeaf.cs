using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class AnyAddressSubdigestLeaf : Leaf
    {
        public const string type = "any-address-subdigest";
        public byte[] digest;
        
        public override object Parse()
        {
            return new
            {
                type = "any-address-subdigest",
                digest = digest.ByteArrayToHexString()
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            var flag = Topology.FlagSignatureAnyAddressSubdigest << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), digest);
        }

        public override byte[] HashConfiguration()
        {
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence any address subdigest:\n");
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, digest));
        }
    }
}