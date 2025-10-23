using System;
using System.Collections.Generic;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class BranchParser
    {
        public static (RecoveryTopology[] topologies, byte[] leftover) ParseBranch(byte[] encoded)
        {
            if (encoded.Length == 0)
                throw new ArgumentException("Empty branch");

            var nodes = new List<RecoveryTopology>();
            int index = 0;

            while (index < encoded.Length)
            {
                var flag = encoded[index];
                if (flag == RecoveryTopology.FlagLeaf)
                {
                    if (encoded.Length < index + 32)
                        throw new ArgumentException("Invalid recovery leaf");

                    var signerBytes = encoded[(index + 1)..(index + 21)];
                    var signer = new Address(signerBytes.ByteArrayToHexStringWithPrefix());

                    var requiredDeltaTimeBytes = encoded[(index + 21)..(index + 24)];
                    var requiredDeltaTime = requiredDeltaTimeBytes.ToBigInteger();

                    var minTimestampBytes = encoded[(index + 24)..(index + 32)];
                    var minTimestamp = minTimestampBytes.ToBigInteger();

                    nodes.Add(new RecoveryTopology(new RecoveryLeaf
                    {
                        signer = signer,
                        requiredDeltaTime = requiredDeltaTime,
                        minTimestamp = minTimestamp
                    }));

                    index += 32;
                    continue;
                }
                
                if (flag == RecoveryTopology.FlagNode)
                {
                    if (encoded.Length < index + 33)
                        throw new ArgumentException("Invalid node");

                    var nodeBytes = encoded[(index + 1)..(index + 33)];
                    nodes.Add(new RecoveryTopology(new RecoveryNode { Value = nodeBytes}));

                    index += 33;
                    continue;
                }
                
                if (flag == RecoveryTopology.FlagBranch)
                {
                    if (encoded.Length < index + 4)
                        throw new ArgumentException("Invalid branch");

                    var sizeBytes = encoded[(index + 1)..(index + 4)];
                    int size = sizeBytes.ToInteger();

                    if (encoded.Length < index + 4 + size)
                        throw new ArgumentException("Invalid branch");

                    var branch = encoded[(index + 4)..(index + 4 + size)];
                    var (subNodes, leftover) = ParseBranch(branch);

                    if (leftover.Length > 0)
                        throw new ArgumentException("Leftover bytes in sub-branch");

                    var subTree = FoldNodes(subNodes);
                    nodes.Add(subTree);

                    index += 4 + size;
                    continue;
                }
                
                throw new ArgumentException("Invalid flag");
            }

            var leftoverBytes = encoded[index..];
            return (nodes.ToArray(), leftoverBytes);
        }

        public static RecoveryTopology FoldNodes(RecoveryTopology[] nodes)
        {
            if (nodes.Length == 0)
                throw new ArgumentException("Empty signature tree");

            if (nodes.Length == 1)
                return nodes[0];

            var tree = nodes[0];
            for (var i = 1; i < nodes.Length; i++)
                tree = new RecoveryTopology(new RecoveryBranch(tree, nodes[i]));

            return tree;
        }
    }
}