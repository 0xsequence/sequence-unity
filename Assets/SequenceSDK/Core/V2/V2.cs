using Sequence.Provider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence;

// Sequence v2 core primitives
namespace Sequence.V2
{
    public class Core : ICore
    {
        public static readonly string nestedLeafImageHashPrefix = "Sequence nested config:\n";
        public static readonly string subdigestLeafImageHashPrefix = "Sequence static digest:\n";

        //Signatures
        public ISignature DecodeSignature(byte[] data)
        {
            if (data.Length == 0)
            {
                throw new ArgumentException("Missing signature type");
            }

            switch ((SignatureType)data[0])
            {
                case SignatureType.Legacy:
                case SignatureType.Regular:
                    return DecodeRegularSignature(data);

                case SignatureType.NoChainID:
                    return DecodeNoChainIDSignature(data);

                case SignatureType.Chained:
                    return DecodeChainedSignature(data);

                default:
                    throw new ArgumentException($"Unknown signature type {data[0]}");
            }
        }


        public static RegularSignature DecodeRegularSignature(byte[] data)
        {
            throw new NotImplementedException();
        }

        public static NoChainIDSignature DecodeNoChainIDSignature(byte[] data)
        {
            throw new NotImplementedException();
        }

        public static ChainedSignature DecodeChainedSignature(byte[] data)
        {
            throw new NotImplementedException();
        }

        private ISignatureTree DecodeSignatureTree(byte[] data)
        {
            ISignatureTree tree = null;

            while (data.Length != 0)
            {
                ISignatureTree leaf = null;
                string err = null;

                switch (data[0])
                {
                    case (byte)SignatureLeafType.ECDSASignature:
                        (leaf, err) = DecodeSignatureTreeECDSASignatureLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode ecdsa signature leaf: {err}");
                        break;

                    case (byte)SignatureLeafType.Address:
                        (leaf, err) = DecodeSignatureTreeAddressLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode address leaf: {err}");
                        break;

                    case (byte)SignatureLeafType.DynamicSignature:
                        (leaf, err) = DecodeSignatureTreeDynamicSignatureLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode dynamic signature leaf: {err}");
                        break;

                    case (byte)SignatureLeafType.Node:
                        (leaf, err) = DecodeSignatureTreeNodeLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode node leaf: {err}");
                        break;

                    case (byte)SignatureLeafType.Branch:
                        (leaf, err) = DecodeSignatureTreeBranchLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode branch leaf: {err}");
                        break;

                    case (byte)SignatureLeafType.Subdigest:
                        (leaf, err) = DecodeSignatureTreeSubdigestLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode subdigest leaf: {err}");
                        break;

                    case (byte)SignatureLeafType.Nested:
                        (leaf, err) = DecodeSignatureTreeNestedLeaf(ref data);
                        if (err != null)
                            throw new Exception($"unable to decode nested leaf: {err}");
                        break;

                    default:
                        throw new Exception($"unknown signature leaf type {data[0]}");
                }

                if (tree == null)
                {
                    tree = leaf;
                }
                else
                {
                    tree = new SignatureTreeNode { left = tree, right = leaf };
                }
            }

            return tree;
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeNestedLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeSubdigestLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeBranchLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeNodeLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeDynamicSignatureLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeAddressLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private (ISignatureTree leaf, string err) DecodeSignatureTreeECDSASignatureLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }


        //Wallet Config
        public WalletConfig DecodeWalletConfig(object obj)
        {
            if (!(obj is Dictionary<string, object> objectMap))
            {
                throw new ArgumentException("Wallet config must be an object");
            }

            if (!objectMap.TryGetValue("threshold", out object thresholdObj) || !(thresholdObj is int threshold))
            {
                throw new ArgumentException("Missing required 'threshold' property");
            }

            if (!objectMap.TryGetValue("checkpoint", out object checkpointObj) || !(checkpointObj is int checkpoint))
            {
                throw new ArgumentException("Missing required 'checkpoint' property");
            }

            if (!objectMap.TryGetValue("tree", out object treeObj) || !(treeObj is Dictionary<string, object> treeDict))
            {
                throw new ArgumentException("Missing required 'tree' property");
            }

            IWalletConfigTree tree = DecodeWalletConfigTree(treeDict);

            return new WalletConfig
            {

            };
        }

        public static IWalletConfigTree DecodeWalletConfigTree(object obj)
        {
            if (obj is not Dictionary<string, object> objectDict)
            {
                throw new ArgumentException("Wallet config tree must be an object");
            }

            if (HasKeys(objectDict, new string[] { "left", "right" }))
            {
                return DecodeWalletConfigTreeNode(objectDict);
            }
            else if (HasKeys(objectDict, new string[] { "weight", "address" }))
            {
                return DecodeWalletConfigTreeAddressLeaf(objectDict);
            }
            else if (HasKeys(objectDict, new string[] { "node" }))
            {
                return DecodeWalletConfigTreeNodeLeaf(objectDict);
            }
            else if (HasKeys(objectDict, new string[] { "weight", "threshold", "tree" }))
            {
                return DecodeWalletConfigTreeNestedLeaf(objectDict);
            }
            else if (HasKeys(objectDict, new string[] { "subdigest" }))
            {
                return DecodeWalletConfigTreeSubdigestLeaf(objectDict);
            }
            else
            {
                throw new ArgumentException("Unknown wallet config tree type");
            }
        }

        private static bool HasKeys(Dictionary<string, object> objectDict, string[] type)
        {
            throw new NotImplementedException();
        }

        private static WalletConfigTreeNode DecodeWalletConfigTreeNode(Dictionary<string, object> obj)
        {
            var node = new WalletConfigTreeNode();
            node.Left = DecodeWalletConfigTree(obj["left"]);
            node.Right = DecodeWalletConfigTree(obj["right"]);
            return node;
        }

        private static WalletConfigTreeAddressLeaf DecodeWalletConfigTreeAddressLeaf(Dictionary<string, object> obj)
        {
            return new WalletConfigTreeAddressLeaf
            {
                Weight = obj["weight"].ToString(),
                Address = obj["address"].ToString()
            };
        }

        private static WalletConfigTreeNodeLeaf DecodeWalletConfigTreeNodeLeaf(Dictionary<string, object> obj)
        {
            return new WalletConfigTreeNodeLeaf
            {
                Node = DecodeWalletConfigTree(obj["node"])
            };
        }

        private static WalletConfigTreeNestedLeaf DecodeWalletConfigTreeNestedLeaf(Dictionary<string, object> obj)
        {
            return new WalletConfigTreeNestedLeaf
            {
                Weight = obj["weight"].ToString(),
                Threshold = Convert.ToUInt16(obj["threshold"]),
                Tree = DecodeWalletConfigTree(obj["tree"])
            };

        }
        private static WalletConfigTreeSubdigestLeaf DecodeWalletConfigTreeSubdigestLeaf(Dictionary<string, object> obj)
        {
            return new WalletConfigTreeSubdigestLeaf
            {
                Subdigest = obj["subdigest"].ToString()
            };
        }


    }

    public enum ECDSASignatureType
    {
        ECDSASignatureTypeEIP712 = 1,
        ECDSASignatureTypeEthSign = 2
    }
    public enum DynamicSignatureType : byte
    {
        DynamicSignatureTypeEIP712 = 1,
        DynamicSignatureTypeEthSign = 2,
        DynamicSignatureTypeEIP1271 = 3
    }

    public interface ISignatureTree
    {
        (WalletConfigTree, BigInteger) Recover(WalletContext context,
                                            Subdigest subdigest,
                                            RPCProvider provider,
                                            List<SignerSignatures> signerSignatures);
    }



    public class SignatureTreeNode : ISignatureTree
    {
        public ISignatureTree left;
        public ISignatureTree right;

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }

    public class SignatureTreeECDSASignatureLeaf : ISignatureTree
    {
        public static int SignatureLength = 0;//Length
        public int weight;
        public ECDSASignatureType type;
        public byte[] signature = new byte[SignatureLength];

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }

    public class SignatureTreeAddressLeaf : ISignatureTree
    {
        public int weight;
        public string address;

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }

    public class SignatureTreeDynamicSignatureLeaf : ISignatureTree
    {
        public int weight;
        public string address;
        public DynamicSignatureType type;
        public byte[] signature;

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }

    public class signatureTreeNodeLeaf : ISignatureTree
    {
        public ImageHash imageHash { get; set; }

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }
    public class SignatureTreeSubdigestLeaf : ISignatureTree
    {
        public Subdigest subdigest { get; set; }

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }

    public class SignatureTreeNestedLeaf : ISignatureTree
    {
        public int weight;
        public int threshold;
        public ISignatureTree tree;

        public (WalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }


    public class RegularSignature : ISignature
    {
        public bool IsRegular { get; set; }
        public ISignatureTree Tree { get; set; }

        public BigInteger Checkpoint()
        {
            throw new NotImplementedException();
        }

        public byte[] Data()
        {
            throw new NotImplementedException();
        }

        public (WalletConfig, BigInteger) Recover(WalletContext context, Digest digest, string walletAddress, BigInteger chainId, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }

        public int Threshold()
        {
            throw new NotImplementedException();
        }
    }

    public class NoChainIDSignature : ISignature
    {
        public BigInteger Checkpoint()
        {
            throw new NotImplementedException();
        }

        public byte[] Data()
        {
            throw new NotImplementedException();
        }

        public (WalletConfig, BigInteger) Recover(WalletContext context, Digest digest, string walletAddress, BigInteger chainId, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }

        public int Threshold()
        {
            throw new NotImplementedException();
        }
    }

    public class ChainedSignature : ISignature
    {
        public BigInteger Checkpoint()
        {
            throw new NotImplementedException();
        }

        public byte[] Data()
        {
            throw new NotImplementedException();
        }

        public (WalletConfig, BigInteger) Recover(WalletContext context, Digest digest, string walletAddress, BigInteger chainId, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }

        public int Threshold()
        {
            throw new NotImplementedException();
        }
    }
    public struct SignerSignature
    {
        public string SignerAddress { get; set; }
        public SignerSignatureType Type { get; set; }
        public byte[] Signature { get; set; }
    }

    public enum SignatureType
    {
        Legacy = 0,
        Regular = 1,
        NoChainID = 2,
        Chained = 3
    }
    public enum SignatureLeafType
    {
        ECDSASignature = 0,
        Address = 1,
        DynamicSignature = 2,
        Node = 3,
        Branch = 4,
        Subdigest = 5,
        Nested = 6
    }

    //WalletConfig 

    public class WalletConfigTree : IWalletConfigTree
    {

    }





    public interface IWalletConfigTree { }

    public class WalletConfigTreeNode : IWalletConfigTree
    {
        public IWalletConfigTree Left { get; set; }
        public IWalletConfigTree Right { get; set; }
    }

    public class WalletConfigTreeAddressLeaf : IWalletConfigTree
    {
        public string Weight { get; set; }
        public string Address { get; set; }
    }

    public class WalletConfigTreeNodeLeaf : IWalletConfigTree
    {
        public IWalletConfigTree Node { get; set; }
    }

    public class WalletConfigTreeNestedLeaf : IWalletConfigTree
    {
        public string Weight { get; set; }
        public ushort Threshold { get; set; }
        public IWalletConfigTree Tree { get; set; }
    }

    public class WalletConfigTreeSubdigestLeaf : IWalletConfigTree
    {
        public string Subdigest { get; set; }
    }


}