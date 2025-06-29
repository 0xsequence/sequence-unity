using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Utils
{
    public static class SignatureUtils
    { 
        public static string EncodeSignatureFromInput(string input, string signatures, bool noChainId, byte[] checkpointerData = null)
        {
            var parts = string.IsNullOrEmpty(signatures) ? 
                Array.Empty<string>() : 
                signatures.Split(' ');
            
            var config = Primitives.Config.FromJson(input);
            
            var allSignatures = parts.Select(s =>
            {
                var values = s.Split(':');
                
                return new
                {
                    Address = new Address(values[0]),
                    Type = values[1],
                    Values = values.Skip(2).ToArray()
                };
            }).ToList();

            var fullTopology = SignatureHandler.FillLeaves(config.topology, leaf =>
            {
                if (leaf is SignerLeaf signerLeaf)
                {
                    var candidate = allSignatures.FirstOrDefault(s => s.Address.Equals(signerLeaf.address));
                    if (candidate == null) 
                        return null;
                    
                    switch (candidate.Type)
                    {
                        case "erc1271":
                            return new SignatureOfSignerLeafErc1271
                            {
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray()
                            };
                        case "eth_sign":
                            return new SignatureOfSignerLeafEthSign
                            {
                                rsy = new RSY
                                {
                                    r = candidate.Values[0].HexStringToBigInteger(),
                                    s = candidate.Values[1].HexStringToBigInteger(),
                                    yParity = RSY.VToYParity(int.Parse(candidate.Values[2]))
                                }
                            };
                        case "hash":
                            return new SignatureOfSignerLeafHash
                            {
                                rsy = new RSY
                                {
                                    r = candidate.Values[0].HexStringToBigInteger(),
                                    s = candidate.Values[1].HexStringToBigInteger(),
                                    yParity = RSY.VToYParity(int.Parse(candidate.Values[2]))
                                }
                            };
                        case "sapient":
                            return new SignatureOfSapientSignerLeaf
                            {
                                curType = SignatureOfSapientSignerLeaf.Type.sapient,
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray()
                            };
                        case "sapient_compact":
                            return new SignatureOfSapientSignerLeaf
                            {
                                curType = SignatureOfSapientSignerLeaf.Type.sapient_compact,
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray()
                            };
                        default:
                            throw new Exception($"Unsupported signature type: {candidate.Type}");
                    }
                }

                if (leaf is SapientSignerLeaf sapientSignerLeaf)
                {
                    var candidate = allSignatures.FirstOrDefault(s => s.Address.Equals(sapientSignerLeaf.address));
                    if (candidate == null) 
                        return null;

                    switch (candidate.Type)
                    {
                        case "sapient":
                        case "sapient_compact":
                            return new SignatureOfSapientSignerLeaf
                            {
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray(),
                            };
                        case "eth_sign":
                        case "hash":
                        case "erc1271":
                            throw new Exception($"Incorrect type for leaf");
                        default:
                            throw new Exception($"Unsupported signature type: {candidate.Type}");
                    }
                }

                return null;
            });

            var rawSignature = new RawSignature
            {
                noChainId = noChainId,
                configuration = new Primitives.Config
                {
                    threshold = config.threshold,
                    checkpoint = config.checkpoint,
                    checkpointer = config.checkpointer,
                    topology = fullTopology
                },
                checkpointerData = checkpointerData
            };

            return rawSignature.Encode().ByteArrayToHexStringWithPrefix();
        }
        
        public static (Topology[] nodes, byte[] Leftover) ParseBranch(byte[] signature)
        {
            var leafs = new List<Topology>();
            int index = 0;

            while (index < signature.Length)
            {
                byte firstByte = signature[index++];
                int flag = (firstByte & 0xf0) >> 4;
                
                switch (flag)
                {
                    case Topology.FlagSignatureHash:
                    {
                        int weight = firstByte & 0x0f;
                        if (weight == 0)
                            weight = signature[index++];

                        if (index + 64 > signature.Length)
                            throw new Exception("Not enough bytes for hash signature");

                        var rsy = RSY.Unpack(signature[index..(index + 64)]);
                        index += 64;

                        leafs.Add(new Topology(new SignedSignerLeaf
                        {
                            weight = new BigInteger(weight),
                            signature = new SignatureOfSignerLeafHash
                            {
                                rsy = rsy
                            }
                        }));
                        
                        break;
                    }

                    case Topology.FlagAddress:
                    {
                        int weight = firstByte & 0x0f;
                        if (weight == 0)
                            weight = signature[index++];
                        
                        string address = signature[index..(index + 20)].ByteArrayToHexStringWithPrefix();
                        index += 20;

                        leafs.Add(new Topology(new SignerLeaf
                        {
                            address = new Address(address),
                            weight = new BigInteger(weight)
                        }));

                        break;
                    }

                    case Topology.FlagSignatureErc1271:
                    {
                        int weight = firstByte & 0x03;
                        if (weight == 0)
                            weight = signature[index++];

                        string signer = signature[index..(index + 20)].ByteArrayToHexStringWithPrefix();
                        index += 20;

                        int sizeSize = (firstByte & 0x0c) >> 2;
                        int dataSize = signature[index..(index + sizeSize)].ToInteger();
                        index += sizeSize;

                        byte[] data = signature[index..(index + dataSize)];
                        index += dataSize;

                        leafs.Add(new Topology(new SignedSignerLeaf
                        {
                            weight = new BigInteger(weight),
                            signature = new SignatureOfSignerLeafErc1271
                            {
                                address = new Address(signer),
                                data = data
                            }
                        }));

                        break;
                    }

                    case Topology.FlagNode:
                    {
                        byte[] nodeHash = signature[index..(index + 32)];
                        index += 32;
                        
                        leafs.Add(new Topology(new NodeLeaf
                        {
                            Value = nodeHash
                        }));
                        break;
                    }

                    case Topology.FlagBranch:
                    {
                        int sizeSize = firstByte & 0x0f;
                        int size = signature[index..(index + sizeSize)].ToInteger();
                        index += sizeSize;

                        var branchBytes = signature[index..(index + size)];
                        index += size;

                        var (subNodes, leftover) = ParseBranch(branchBytes);
                        if (leftover.Length > 0)
                            throw new Exception("Leftover bytes in sub-branch");

                        leafs.Add(FoldNodes(subNodes));
                        break;
                    }

                    case Topology.FlagSubdigest:
                    {
                        byte[] digest = signature[index..(index + 32)];
                        index += 32;
                        leafs.Add(new Topology(new SubdigestLeaf
                        {
                            digest = digest
                        }));
                        break;
                    }

                    case Topology.FlagNested:
                    {
                        int externalWeight = (firstByte & 0x0c) >> 2;
                        if (externalWeight == 0)
                            externalWeight = signature[index++];

                        int internalThreshold = firstByte & 0x03;
                        if (internalThreshold == 0)
                        {
                            internalThreshold = signature[index..(index + 2)].ToInteger();
                            index += 2;
                        }

                        int size = signature[index..(index + 3)].ToInteger();
                        index += 3;

                        var nestedBytes = signature[index..(index + size)];
                        index += size;

                        var (subNodes, leftover) = ParseBranch(nestedBytes);
                        if (leftover.Length > 0)
                            throw new Exception("Leftover bytes in nested tree");

                        leafs.Add(new Topology(new NestedLeaf
                        {
                            tree = FoldNodes(subNodes),
                            weight = new BigInteger(externalWeight),
                            threshold = new BigInteger(internalThreshold)
                        }));

                        break;
                    }

                    case Topology.FlagSignatureEthSign:
                    {
                        int weight = firstByte & 0x0f;
                        if (weight == 0)
                            weight = signature[index++];

                        var rsy = RSY.Unpack(signature[index..(index + 64)]);
                        index += 64;

                        leafs.Add(new Topology(new SignedSignerLeaf
                        {
                            weight = new BigInteger(weight),
                            signature = new SignatureOfSignerLeafEthSign
                            {
                                rsy = rsy
                            }
                        }));

                        break;
                    }

                    case Topology.FlagSignatureAnyAddressSubdigest:
                    {
                        byte[] digest = signature[index..(index + 32)];
                        index += 32;

                        leafs.Add(new Topology(new AnyAddressSubdigestLeaf
                        {
                            digest = digest,
                        }));
                        break;
                    }

                    case Topology.FlagSignatureSapient:
                    case Topology.FlagSignatureSapientCompact:
                    {
                        int weight = firstByte & 0x03;
                        if (weight == 0)
                            weight = signature[index++];

                        string address = signature[index..(index + 20)].ByteArrayToHexStringWithPrefix();
                        index += 20;

                        int sizeSize = (firstByte & 0x0c) >> 2;
                        int dataSize = signature[index..(index + sizeSize)].ToInteger();
                        index += sizeSize;

                        byte[] data = signature[index..(index + dataSize)];
                        index += dataSize;

                        leafs.Add(new Topology(new SignedSapientSignerLeaf
                        {
                            weight = new BigInteger(weight),
                            signature = new SignatureOfSapientSignerLeaf
                            {
                                curType = flag == Topology.FlagSignatureSapient ? SignatureOfSapientSignerLeaf.Type.sapient : SignatureOfSapientSignerLeaf.Type.sapient_compact,
                                address = new Address(address),
                                data = data
                            }
                        }));

                        break;
                    }

                    default:
                        throw new Exception($"Invalid signature flag: 0x{flag:X}");
                }
            }
            
            return (leafs.ToArray(), signature[index..]);
        }
        
        public static Topology FoldNodes(Topology[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
                throw new Exception("Empty signature tree");

            if (nodes.Length == 1)
                return nodes[0];

            var tree = nodes[0];
            for (var i = 1; i < nodes.Length; i++)
            {
                var node = new Node(tree, nodes[i]);
                tree = new Topology(node);
            }

            return tree;
        }
    }
}