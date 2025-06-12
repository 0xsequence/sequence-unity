using System;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class Topology
    {
        public Node Node;
        public Leaf Leaf;

        public Topology(Leaf leaf)
        {
            this.Leaf = leaf;
        }

        public Topology(Node node)
        {
            this.Node = node;
        }

        public bool IsLeaf()
        {
            return this.Leaf != null;
        }

        public bool IsNode()
        {
            return this.Node != null;
        }
        
        public Leaf FindSignerLeaf(Address address)
        {
            if (IsNode())
            {
                Leaf leftResult = Node.left.FindSignerLeaf(address);
                if (leftResult != null)
                {
                    return leftResult;
                }
                return Node.right.FindSignerLeaf(address);
            }
            else if (IsLeaf())
            {
                if (Leaf.isSignerLeaf)
                {
                    SignerLeaf signerLeaf = (SignerLeaf)Leaf;
                    if (signerLeaf.address.Equals(address))
                    {
                        return signerLeaf;
                    }
                }
                else if (Leaf.isSapientSignerLeaf)
                {
                    SapientSignerLeaf sapientLeaf = (SapientSignerLeaf)Leaf;
                    if (sapientLeaf.address.Equals(address))
                    {
                        return sapientLeaf;
                    }
                }
            }
            return null;
        }

        // Todo once tests are passing refactor to use a HashConfiguration method on the leafs specifically, we can add an abstract method to be overwritten
        public byte[] HashConfiguration()
        {
            if (IsNode())
            {
                byte[] leftHash = Node.left.HashConfiguration();
                byte[] rightHash = Node.right.HashConfiguration();
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(leftHash, rightHash));
            }
            
            if (Leaf.isSignerLeaf)
            {
                SignerLeaf signerLeaf = (SignerLeaf)Leaf;
                byte[] prefix = Encoding.UTF8.GetBytes("Sequence signer:\n");
                byte[] address = signerLeaf.address.Value.HexStringToByteArray();
                byte[] weight = signerLeaf.weight.ByteArrayFromNumber().PadLeft(32);
                
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, address, weight));
            }
            
            if (Leaf.isSapientSignerLeaf)
            {
                SapientSignerLeaf sapientLeaf = (SapientSignerLeaf)Leaf;
                byte[] prefix = Encoding.UTF8.GetBytes("Sequence sapient config:\n");
                byte[] address = sapientLeaf.address.Value.HexStringToByteArray();
                byte[] weight = sapientLeaf.weight.ByteArrayFromNumber().PadLeft(32);
                byte[] imageHash = sapientLeaf.imageHash.HexStringToByteArray().PadLeft(32);
                
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, address, weight, imageHash));
            }
            
            if (Leaf.isSubdigestLeaf)
            {
                SubdigestLeaf subdigestLeaf = (SubdigestLeaf)Leaf;
                byte[] prefix = Encoding.UTF8.GetBytes("Sequence static digest:\n");
                byte[] digest = subdigestLeaf.digest;
                
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, digest));
            }
            
            if (Leaf.isAnyAddressSubdigestLeaf)
            {
                AnyAddressSubdigestLeaf anyAddressSubdigestLeaf = (AnyAddressSubdigestLeaf)Leaf;
                byte[] prefix = Encoding.UTF8.GetBytes("Sequence any address subdigest:\n");
                byte[] digest = anyAddressSubdigestLeaf.digest;
                
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, digest));
            }
            
            if (Leaf.isNodeLeaf)
            {
                // In the JS code, this just returns the topology itself, but in C# we need to return bytes
                // Since NodeLeaf doesn't have any properties to hash, we'll return a byte array
                return new byte[]{};
            }
            
            if (Leaf.isNestedLeaf)
            {
                NestedLeaf nestedLeaf = (NestedLeaf)Leaf;
                byte[] prefix = Encoding.UTF8.GetBytes("Sequence nested config:\n");
                byte[] treeHash = nestedLeaf.tree.HashConfiguration();
                byte[] threshold = nestedLeaf.threshold.ByteArrayFromNumber().PadLeft(32);
                byte[] weight = nestedLeaf.weight.ByteArrayFromNumber().PadLeft(32);
                
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, treeHash, threshold, weight));
            }
            
            throw new InvalidOperationException($"Invalid topology, given {GetType()}");
        }
    }
}