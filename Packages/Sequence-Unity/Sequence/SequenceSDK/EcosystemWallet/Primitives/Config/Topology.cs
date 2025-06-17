using System;
using System.Numerics;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class Topology
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

        public static Topology FromLeaves(List<Leaf> leaves)
        {
            if (leaves == null || leaves.Count == 0)
            {
                throw new ArgumentException("Cannot create topology from empty leaves");
            }

            if (leaves.Count == 1)
            {
                return new Topology(leaves[0]);
            }

            if (leaves.Count == 2)
            {
                return new Topology(new Node(
                    new Topology(leaves[0]),
                    new Topology(leaves[1])
                ));
            }

            int midPoint = leaves.Count / 2;
            return new Topology(new Node(
                FromLeaves(leaves.GetRange(0, midPoint)),
                FromLeaves(leaves.GetRange(midPoint, leaves.Count - midPoint))
            ));
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

        // Todo refactor
        public object Encode()
        {
            if (IsNode())
            {
                return new object[]
                {
                    Node.left.Encode(),
                    Node.right.Encode()
                };
            }
            else if (Leaf.isSignerLeaf)
            {
                SignerLeaf signerLeaf = (SignerLeaf)Leaf;
                return new
                {
                    type = "signer",
                    address = signerLeaf.address,
                    weight = signerLeaf.weight.ToString()
                };
            }
            else if (Leaf.isSapientSignerLeaf)
            {
                SapientSignerLeaf sapientLeaf = (SapientSignerLeaf)Leaf;
                return new
                {
                    type = "sapient-signer",
                    address = sapientLeaf.address,
                    weight = sapientLeaf.weight.ToString(),
                    imageHash = sapientLeaf.imageHash
                };
            }
            else if (Leaf.isSubdigestLeaf)
            {
                SubdigestLeaf subdigestLeaf = (SubdigestLeaf)Leaf;
                return new
                {
                    type = "subdigest",
                    digest = subdigestLeaf.digest.ByteArrayToHexString()
                };
            }
            else if (Leaf.isAnyAddressSubdigestLeaf)
            {
                AnyAddressSubdigestLeaf anyAddressSubdigestLeaf = (AnyAddressSubdigestLeaf)Leaf;
                return new
                {
                    type = "any-address-subdigest",
                    digest = anyAddressSubdigestLeaf.digest.ByteArrayToHexString()
                };
            }
            else if (Leaf.isNodeLeaf)
            {
                NodeLeaf nodeLeaf = (NodeLeaf)Leaf;
                return nodeLeaf.Value.ByteArrayToHexString();
            }
            else if (Leaf.isNestedLeaf)
            {
                NestedLeaf nestedLeaf = (NestedLeaf)Leaf;
                return new
                {
                    type = "nested",
                    tree = nestedLeaf.tree.Encode(),
                    weight = nestedLeaf.weight.ToString(),
                    threshold = nestedLeaf.threshold.ToString()
                };
            }

            throw new InvalidOperationException("Invalid topology");
        }

        public static Topology Decode(string input)
        {
            if (input.StartsWith("["))
            {
                var list = JsonConvert.DeserializeObject<List<object>>(input);
                if (list.Count != 2)
                {
                    throw new Exception("Invalid node structure in JSON");
                }

                return new Topology(new Node(Decode(list[0].ToString()), Decode(list[1].ToString())));
            }

            // Case: string → leaf node (digest hash or node ID)
            /*if (input.IsHexFormat())
            {
                Leaf
                return hex;
            }*/

            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);
            string type = (string)data["type"];
            Leaf leaf;
            
            switch (type)
            {
                case "signer":
                    leaf = new SignerLeaf
                    {
                        address = new Address((string)data["address"]),
                        weight = BigInteger.Parse((string)data["weight"])
                    };

                    break;
                case "sapient-signer":
                    leaf = new SapientSignerLeaf
                    {
                        address = new Address((string)data["address"]),
                        weight = BigInteger.Parse((string)data["weight"]),
                        imageHash = (string)data["imageHash"]
                    };

                    break;
                case "subdigest":
                    leaf = new SubdigestLeaf
                    {
                        digest = ((string)data["digest"]).HexStringToByteArray()
                    };
                    
                    break;
                case "any-address-subdigest":
                    leaf = new AnyAddressSubdigestLeaf
                    {
                        digest = ((string)data["digest"]).HexStringToByteArray()
                    };

                    break;
                case "nested":
                    leaf = new NestedLeaf
                    {
                        tree = FromLeaves(null),
                        weight = BigInteger.Parse((string)data["weight"]),
                        threshold = BigInteger.Parse((string)data["threshold"])
                    };

                    break;
                default:
                    throw new Exception("Invalid type in topology JSON");
            }

            return new Topology(leaf);
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