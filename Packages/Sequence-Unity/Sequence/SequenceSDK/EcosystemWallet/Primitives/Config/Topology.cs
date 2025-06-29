using System;
using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine;

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
            
            return Leaf.Parse();
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

            if (!input.StartsWith("{"))
            {
                return new Topology(new NodeLeaf
                {
                    Value = (input).HexStringToByteArray()
                });
            }

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
                        tree = Decode(data["tree"].ToString()),
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
            
            return Leaf.HashConfiguration();
        }
        
        public byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            if (IsNode())
            {
                var encoded0 = Node.left.Encode(noChainId, checkpointerData);
                var encoded1 = Node.right.Encode(noChainId, checkpointerData);
                
                var isBranching = Node.right.IsNode();
                if (!isBranching) 
                    return ByteArrayExtensions.ConcatenateByteArrays(encoded0, encoded1);
                
                var encoded1Size = encoded1.Length.MinBytesFor();
                if (encoded1Size > 15) 
                    throw new Exception("Branch too large");
                
                var flag = (FlagBranch << 4) | encoded1Size;
                
                return ByteArrayExtensions.ConcatenateByteArrays(encoded0, 
                    flag.ByteArrayFromNumber(flag.MinBytesFor()),
                    encoded1.Length.ByteArrayFromNumber(encoded1Size)
                        .PadLeft(encoded1Size),
                    encoded1);
            }

            return Leaf.Encode(noChainId, checkpointerData);
        }
        
        public static List<Leaf> ParseContentToLeafs(string elements)
        {
            var leaves = new List<Leaf>();

            while (!string.IsNullOrWhiteSpace(elements))
            {
                string firstElement = elements.Split(' ')[0];
                string firstElementType = firstElement.Split(':')[0];

                if (firstElementType == "signer")
                {
                    var parts = firstElement.Split(':');
                    string address = parts[1];
                    BigInteger weight = BigInteger.Parse(parts[2]);

                    leaves.Add(new SignerLeaf
                    {
                        address = new  Address(address),
                        weight = weight
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "subdigest")
                {
                    var parts = firstElement.Split(':');
                    string digest = parts[1];

                    leaves.Add(new SubdigestLeaf
                    {
                        digest = digest.HexStringToByteArray()
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "any-address-subdigest")
                {
                    var parts = firstElement.Split(':');
                    string digest = parts[1];

                    leaves.Add(new AnyAddressSubdigestLeaf
                    {
                        digest = digest.HexStringToByteArray()
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "sapient")
                {
                    var parts = firstElement.Split(':');
                    string imageHash = parts[1];
                    string address = parts[2];
                    BigInteger weight = BigInteger.Parse(parts[3]);

                    if (!imageHash.StartsWith("0x") || imageHash.Length != 66)
                        throw new Exception($"Invalid image hash: {imageHash}");

                    leaves.Add(new SapientSignerLeaf
                    {
                        imageHash = imageHash,
                        address = new Address(address),
                        weight = weight
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "nested")
                {
                    var parts = firstElement.Split(':');
                    BigInteger threshold = BigInteger.Parse(parts[1]);
                    BigInteger weight = BigInteger.Parse(parts[2]);

                    int start = elements.IndexOf('(');
                    int end = elements.IndexOf(')');
                    if (start == -1 || end == -1)
                        throw new Exception($"Missing ( ) for nested element: {elements}");

                    string inner = elements.Substring(start + 1, end - start - 1);

                    leaves.Add(new NestedLeaf
                    {
                        threshold = threshold,
                        weight = weight,
                        tree = Topology.FromLeaves(ParseContentToLeafs(inner))
                    });

                    elements = elements.Substring(end + 1).TrimStart();
                }
                else if (firstElementType == "node")
                {
                    var parts = firstElement.Split(':');
                    string hash = parts[1];

                    leaves.Add(new NodeLeaf
                    {
                        Value = hash.HexStringToByteArray()
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else
                {
                    throw new Exception($"Invalid element: {firstElement}");
                }
            }
            
            if (leaves.Count == 0)
                Debug.LogError($"Topology.FromLeaves resulted in an empty list of leafs: {elements}");

            return leaves;
        }
    }
}