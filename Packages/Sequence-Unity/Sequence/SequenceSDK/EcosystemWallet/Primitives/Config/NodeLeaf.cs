using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class NodeLeaf : Leaf
    {
        public byte[] Value;

        public static implicit operator byte[](NodeLeaf leaf)
        {
            return leaf.Value;
        }

        public override object Parse()
        {
            return Value.ByteArrayToHexString();
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            var flag = Topology.FlagNode << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), Value);
        }

        // In the JS code, this just returns the topology itself, but in C# we need to return bytes
        // Since NodeLeaf doesn't have any properties to hash, we'll return a byte array
        public override byte[] HashConfiguration()
        {
            return new byte[] { };
        }
    }
}