using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class Topology
    {
        public const int FlagSignatureHash = 0;
        public const int FlagAddress = 1;
        public const int FlagSignatureErc1271 = 2;
        public const int FlagNode = 3;
        public const int FlagBranch = 4;
        public const int FlagSubdigest = 5;
        public const int FlagNested = 6;
        public const int FlagSignatureEthSign = 7;
        public const int FlagSignatureAnyAddressSubdigest = 8;
        public const int FlagSignatureSapient = 9;
        public const int FlagSignatureSapientCompact = 10;
        
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
            
            if (IsLeaf())
            {
                if (Leaf is SignerLeaf signerLeaf)
                {
                    if (signerLeaf.address.Equals(address))
                    {
                        return signerLeaf;
                    }
                }
                else if (Leaf is SapientSignerLeaf sapientSignerLeaf)
                {
                    if (sapientSignerLeaf.address.Equals(address))
                    {
                        return sapientSignerLeaf;
                    }
                }
            }
            
            return null;
        }

        public object Parse()
        {
            if (IsNode())
            {
                return new object[]
                {
                    Node.left.Parse(),
                    Node.right.Parse()
                };
            }
            
            if (Leaf is SignerLeaf signerLeaf)
            {
                return signerLeaf.Parse();
            }
            
            if (Leaf is SapientSignerLeaf sapientSignerLeaf)
            {
                return sapientSignerLeaf.Parse();
            }
            
            if (Leaf is SubdigestLeaf subdigestLeaf)
            {
                return subdigestLeaf.Parse();
            }
            
            if (Leaf is AnyAddressSubdigestLeaf anyAddressSubdigestLeaf)
            {
                return anyAddressSubdigestLeaf.Parse();
            }
            
            if (Leaf is NodeLeaf nodeLeaf)
            {
                return nodeLeaf.Value.ByteArrayToHexString();
            }
            
            if (Leaf is NestedLeaf nestedLeaf)
            {
                return nestedLeaf.Parse();
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

        public byte[] HashConfiguration()
        {
            if (IsNode())
            {
                byte[] leftHash = Node.left.HashConfiguration();
                byte[] rightHash = Node.right.HashConfiguration();
                return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(leftHash, rightHash));
            }
            
            if (Leaf is SignerLeaf signerLeaf)
            {
                return signerLeaf.HashConfiguration();
            }
            
            if (Leaf is SapientSignerLeaf sapientSignerLeaf)
            {
                return sapientSignerLeaf.HashConfiguration();
            }
            
            if (Leaf is SubdigestLeaf subdigestLeaf)
            {
                return subdigestLeaf.HashConfiguration();
            }
            
            if (Leaf is AnyAddressSubdigestLeaf anyAddressSubdigestLeaf)
            {
                return anyAddressSubdigestLeaf.HashConfiguration();
            }
            
            if (Leaf is NodeLeaf nodeLeaf)
            {
                return nodeLeaf.HashConfiguration();
            }
            
            if (Leaf is NestedLeaf nestedLeaf)
            {
                return nestedLeaf.HashConfiguration();
            }
            
            throw new InvalidOperationException($"Invalid topology, given {GetType()}");
        }
        
        public byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            if (IsNode())
            {
                var encoded0 = Node.left.Encode(noChainId, checkpointerData);
                var encoded1 = Node.right.Encode(noChainId, checkpointerData);
                
                var isBranching = Node.right.IsNode();
                if (!isBranching) 
                    return encoded0.Concat(encoded1).ToArray();
                
                var encoded1Size = encoded1.Length.MinBytesFor();
                if (encoded1Size > 15) 
                    throw new Exception("Branch too large");

                int flag = (FlagBranch << 4) | encoded1Size;
                return encoded0
                    .Concat(flag.ByteArrayFromNumber())
                    .Concat(encoded1.Length.ByteArrayFromNumber()
                        .PadLeft(encoded1Size))
                    .Concat(encoded1)
                    .ToArray();
            }

            if (Leaf is NestedLeaf nestedLeaf)
            {
                nestedLeaf.Encode(noChainId, checkpointerData);
            }

            if (Leaf is NodeLeaf nodeLeaf)
            {
                return nodeLeaf.Encode(noChainId, checkpointerData);
            }

            if (Leaf is SignedSignerLeaf signedSignerLeaf)
            {
                signedSignerLeaf.Encode(noChainId, checkpointerData);
            }

            if (Leaf is RawSignerLeaf rawSignerLeaf)
            {
                rawSignerLeaf.Encode(noChainId, checkpointerData);
            }

            throw new Exception("Unrecognized topology structure.");
        }
    }
}