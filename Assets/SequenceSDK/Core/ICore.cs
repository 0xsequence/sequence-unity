using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


namespace Sequence.Core
{
    // Sequence core primitives, inherited from go-sequence v2
    //
    // DecodeSignature takes raw signature data and returns a Signature.
    // A Signature can Recover the WalletConfig it represents.
    // A WalletConfig describes the configuration of signers that control a wallet.
    public interface ICore
    {
        // DecodeSignature takes raw signature data and returns a Signature that can Recover a WalletConfig.
        public ISignature DecodeSignature(byte[] data);

        // DecodeWalletConfig takes a decoded JSON object and returns a WalletConfig.
        public WalletConfig DecodeWalletConfig(object obj);
    }
}
