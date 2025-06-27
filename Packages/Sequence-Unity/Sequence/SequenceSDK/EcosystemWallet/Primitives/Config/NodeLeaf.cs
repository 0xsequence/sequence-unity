using System.Linq;
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
            return (Topology.FlagNode << 4).ByteArrayFromNumber().Concat(Value).ToArray();
        }

        // In the JS code, this just returns the topology itself, but in C# we need to return bytes
        // Since NodeLeaf doesn't have any properties to hash, we'll return a byte array
        public override byte[] HashConfiguration()
        {
            return new byte[]{};
        }
    }
}