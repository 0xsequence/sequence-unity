using System;
using System.Collections.Generic;
using Sequence.Core;

namespace Sequence.Core.Wallet {
    public interface IWalletConfig
    {
        // ImageHash is the digest of the object.
        ImageHash ImageHash();

        // Threshold is the minimum signing weight required for a signature to be valid.
        UInt16 Threshold();

        // Checkpoint is the nonce of the wallet configuration.
        UInt32 Checkpoint();

        // Signers is the set of signers in the wallet configuration.
        Dictionary<string, UInt16> Signers();

        // SignersWeight is the total weight of the signers passed to the function according to the wallet configuration.
        UInt16 SignersWeight(string[] signers);

        // IsUsable checks if it's possible to construct signatures that meet threshold.
        bool IsUsable();
    }
}