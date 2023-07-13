using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sequence.Core.V2.Wallet.ConfigTree;

namespace Sequence.Core.V2.Wallet
{
    public static class WalletConfigTreeDecoder
    {
        public static IWalletConfigTree Decode(object obj)
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
            node.Left = Decode(obj["left"]);
            node.Right = Decode(obj["right"]);
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
                Node = Decode(obj["node"])
            };
        }

        private static WalletConfigTreeNestedLeaf DecodeWalletConfigTreeNestedLeaf(Dictionary<string, object> obj)
        {
            return new WalletConfigTreeNestedLeaf
            {
                Weight = obj["weight"].ToString(),
                Threshold = Convert.ToUInt16(obj["threshold"]),
                Tree = Decode(obj["tree"])
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
}
