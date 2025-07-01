using System;
using System.Linq;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class TopologyExtensions
    {
        public static bool IsBranch(this ITopology topology)
        {
            return topology.Branch != null;
        }
        
        public static bool IsLeaf(this ITopology topology)
        {
            return topology.Leaf != null;
        }
        
        public static bool IsNode(this ITopology topology)
        {
            return topology.Node != null;
        }
        
        public static string Hash(this ITopology topology, bool raw = false)
        {
            if (topology.IsBranch())
            {
                var children = topology.Branch.Children;
                if (children.Length == 0)
                    throw new Exception("Empty branch");

                var hashedChildren = children.Select(child => child.Hash(raw)).ToArray();

                var childBytes = hashedChildren[0].HexStringToByteArray();
                for (var i = 1; i < hashedChildren.Length; i++)
                {
                    var nextBytes = hashedChildren[i].HexStringToByteArray();
                    childBytes = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(childBytes, nextBytes));
                }

                return childBytes.ByteArrayToHexStringWithPrefix();
            }

            if (topology.IsNode())
                return topology.Node.Value.ByteArrayToHexStringWithPrefix();
            
            if (topology.IsLeaf())
            {
                var encodedLeaf = raw ? topology.Leaf.EncodeRaw() : topology.Leaf.Encode();
                return SequenceCoder.KeccakHash(encodedLeaf)
                    .ByteArrayToHexStringWithPrefix();
            }

            throw new Exception("Invalid tree structure");
        }
        
        public static T FindLeaf<T>(this ITopology topology, Func<T, bool> check) where T : ILeaf
        {
            if (topology.Leaf is T leaf && check(leaf))
                return leaf;

            if (topology.Branch == null) 
                return default;

            return topology.Branch.Children.Select(child => child.FindLeaf(check))
                .FirstOrDefault(childLeaf => childLeaf != null);
        }
    }
}