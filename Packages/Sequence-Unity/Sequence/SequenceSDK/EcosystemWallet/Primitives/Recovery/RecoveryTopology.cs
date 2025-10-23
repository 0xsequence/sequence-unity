using System;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryTopology : ITopology
    {
        public const int FlagLeaf = 1;
        public const int FlagNode = 3;
        public const int FlagBranch = 4;
        
        public IBranch Branch { get; }
        public ILeaf Leaf { get; }
        public INode Node { get; }

        public RecoveryTopology(RecoveryBranch branch)
        {
            Branch = branch;
        }
        
        public RecoveryTopology(RecoveryNode node)
        {
            Node = node;
        }
        
        public RecoveryTopology(RecoveryLeaf leaf)
        {
            Leaf = leaf;
        }

        public RecoveryTopology Trim(Address signer)
        {
            if (Leaf is RecoveryLeaf leaf)
            {
                if (leaf.signer.Equals(signer))
                    return this;

                var node = new RecoveryNode { Value = this.Hash(true).HexStringToByteArray() };
                return new RecoveryTopology(node);
            }

            if (this.IsNode())
                return this;

            if (Branch is RecoveryBranch branch)
            {
                var left = branch.Left.Trim(signer);
                var right = branch.Right.Trim(signer);

                if (left.IsNode() && right.IsNode())
                {
                    var node = new RecoveryNode { Value = this.Hash(true).HexStringToByteArray() };
                    return new RecoveryTopology(node);
                }

                return new RecoveryTopology(new RecoveryBranch(left, right));
            }

            throw new ArgumentException("Invalid topology");
        }

        public static RecoveryTopology FromLeaves(RecoveryLeaf[] leaves)
        {
            if (leaves.Length == 0)
                throw new Exception("Empty Leaves");

            if (leaves.Length == 1)
                return new RecoveryTopology(leaves[0]);

            var mid = (int)Math.Floor(leaves.Length / 2f);
            var left = FromLeaves(leaves.SubArray(0, mid));
            var right = FromLeaves(leaves.SubArray(mid));
            return new RecoveryTopology(new RecoveryBranch(left, right));
        }

        public static RecoveryTopology Decode(byte[] encoded)
        {
            var (topologies, leftover) = BranchParser.ParseBranch(encoded);
            if (leftover.Length > 0)
                throw new Exception("There's still leftover");

            return BranchParser.FoldNodes(topologies);
        }
    }
}