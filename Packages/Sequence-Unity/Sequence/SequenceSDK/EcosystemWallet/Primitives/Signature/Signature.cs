using System;

namespace Sequence.EcosystemWallet.Primitives
{
    public class Signature
    {
        public static Topology FillLeaves(Topology topology, Func<Leaf, SignatureType> signatureFor)
        {
            if (topology.IsNode())
            {
                return new Topology(new Node(
                    FillLeaves(topology.Node.left, signatureFor),
                    FillLeaves(topology.Node.right, signatureFor)));
            }
            
            if (!topology.IsLeaf())
                throw new ArgumentException("Cannot create topology from empty leaves");

            if (topology.Leaf.isSignerLeaf)
            {
                var leaf = topology.Leaf as SignerLeaf;
                var signature = signatureFor(leaf);
                var newLeaf = signature != null ? new SignedSignerLeaf
                {
                    address = leaf.address,
                    weight = leaf.weight,
                    signed = true,
                    signature = signature
                } : leaf;
                
                return new Topology(newLeaf);
            }

            if (topology.Leaf.isSapientSignerLeaf)
            {
                var leaf = topology.Leaf as SapientSignerLeaf;
                var signature = signatureFor(leaf);
                var newLeaf = signature != null ? new SignedSapientSignerLeaf
                {
                    address = leaf.address,
                    imageHash = leaf.imageHash,
                    weight = leaf.weight,
                    signed = true,
                    signature = signature
                } : leaf;
                
                return new Topology(newLeaf);
            }

            if (topology.Leaf.isNestedLeaf)
            {
                var nested = topology.Leaf as NestedLeaf;
                return new Topology(new NestedLeaf
                {
                    weight = nested.weight,
                    threshold = nested.threshold,
                    tree = FillLeaves(nested.tree, signatureFor)
                });
            }
            
            if (topology.Leaf.isSubdigestLeaf ||
                topology.Leaf.isAnyAddressSubdigestLeaf ||
                topology.Leaf.isNodeLeaf)
            {
                return topology;
            }

            throw new Exception("Invalid topology");
        }
    }
}