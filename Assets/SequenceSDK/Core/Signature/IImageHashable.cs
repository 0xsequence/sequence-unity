using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence.ABI;
using Sequence.Wallet;
using System.Text;
using System;
using System.Linq;

namespace Sequence.Core.Signature
{
    public interface IImageHashable
    {
        ImageHash ImageHash();
    }

    // An ImageHash is a digest of an ImageHashable.
    // Used for type safety and preimage recovery.
    public class ImageHash
    {
        public Hash Hash { get; set; }

        // Preimage is the ImageHashable with this ImageHash, null if unknown.
        public IImageHashable[] Preimage { get; set; }

        public static readonly string ApprovalSalt = SequenceCoder.KeccakHash("SetImageHash(bytes32 imageHash)");

        // Approval derives the digest that must be signed to approve the ImageHash for subsequent signatures.
        public Digest Approval()
        {           

            return Digest.NewDigest(ApprovalSalt, Hash.ToString());
        }
    }
}
