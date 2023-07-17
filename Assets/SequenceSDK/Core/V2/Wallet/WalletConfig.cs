using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;
using UnityEngine;

namespace Sequence.Core.V2.Wallet
{
    [System.Serializable]
    public class WalletConfig : IWalletConfig
    {
        public UInt16 threshold;
        public UInt32 checkpoint;
        public IWalletConfigTree tree;

        public uint Checkpoint()
        {
            return checkpoint;
        }

        public ImageHash ImageHash()
        {
            throw new NotImplementedException();
        }

        public bool IsUsable()
        {
            throw new NotImplementedException();
        }

        public Dictionary<Address, UInt16> Signers()
        {
            var signers = tree.ReadSignersIntoMap();
            return signers;
        }

        public ushort SignersWeight(Address[] signers)
        {
            Dictionary<Address, UInt16> signersMap = new Dictionary<Address, ushort>();
            int signersLength = signers.Length;
            for (int i = 0; i < signersLength; i++)
            {
                signersMap[signers[i]] = 0;
            }

            BigInteger weight = tree.UnverifiedWeight(signersMap);
            return (UInt16)weight;
        }

        public ushort Threshold()
        {
            return threshold;
        }
    }
}
