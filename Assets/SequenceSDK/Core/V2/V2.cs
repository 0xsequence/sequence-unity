using Sequence.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence.Core.Wallet;
using Sequence.Core.Signature;
using Sequence.Core.V2.Signature;
using Sequence.Core.V2.Wallet;

// Sequence v2 core primitives
namespace Sequence.Core.V2
{
    public class Core : ICore
    {
        public static readonly string nestedLeafImageHashPrefix = "Sequence nested config:\n";
        public static readonly string subdigestLeafImageHashPrefix = "Sequence static digest:\n";

        public enum SignatureType
        {
            Legacy = 0,
            Regular = 1,
            NoChainID = 2,
            Chained = 3
        }

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
                    return SignatureDecoder.DecodeRegularSignature(data);

                case SignatureType.NoChainID:
                    return SignatureDecoder.DecodeNoChainIDSignature(data);

                case SignatureType.Chained:
                    return SignatureDecoder.DecodeChainedSignature(data);

                default:
                    throw new ArgumentException($"Unknown signature type {data[0]}");
            }
        }

        //Wallet Config
        public IWalletConfig DecodeWalletConfig(object obj)
        {
            if (!(obj is Dictionary<string, object> objectMap))
            {
                throw new ArgumentException("Wallet config must be an object");
            }

            if (!objectMap.TryGetValue("threshold", out object thresholdObj) || !(thresholdObj is UInt16 threshold))
            {
                throw new ArgumentException("Missing required 'threshold' property");
            }

            if (!objectMap.TryGetValue("checkpoint", out object checkpointObj) || !(checkpointObj is UInt32 checkpoint))
            {
                throw new ArgumentException("Missing required 'checkpoint' property");
            }

            if (!objectMap.TryGetValue("tree", out object treeObj) || !(treeObj is Dictionary<string, object> treeDict))
            {
                throw new ArgumentException("Missing required 'tree' property");
            }

            IWalletConfigTree tree = WalletConfigTreeDecoder.Decode(treeDict);

            return new WalletConfig(threshold, checkpoint, tree);
        }
    }
}