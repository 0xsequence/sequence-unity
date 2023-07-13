using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core.Signature;
using UnityEngine;

namespace Sequence.Core.V2
{
    public interface IWalletConfigTree
    {
        ImageHash ImageHash();
        BigInteger MaxWeight();
        Dictionary<Address, UInt16> ReadSignersIntoMap();
        BigInteger UnverifiedWeight(Dictionary<Address, UInt16> signers);
        ISignatureTree BuildSignatureTree(Dictionary<Address, SignerSignature> signerSignatures);
    }

    public struct SignerSignature
    {
        public string SignerAddress { get; set; }
        public SignerSignatureType Type { get; set; }
        public byte[] Signature { get; set; }
    }
}
