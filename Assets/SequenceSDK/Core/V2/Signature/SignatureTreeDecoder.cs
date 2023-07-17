using System;
using Sequence.Core.V2.Signature.Tree;

namespace Sequence.Core.V2.Signature
{
    internal static class SignatureTreeDecoder
    {
        private enum SignatureLeafType
        {
            ECDSASignature = 0,
            Address = 1,
            DynamicSignature = 2,
            Node = 3,
            Branch = 4,
            Subdigest = 5,
            Nested = 6
        }

        internal static ISignatureTree DecodeSignatureTree(byte[] data)
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

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeNestedLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeSubdigestLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeBranchLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeNodeLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeDynamicSignatureLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeAddressLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }

        private static (ISignatureTree leaf, string err) DecodeSignatureTreeECDSASignatureLeaf(ref byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
