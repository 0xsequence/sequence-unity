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
        public string Hash { get; set; }
        // Preimage is the ImageHashable with this ImageHash,
        // in go-sequence :
        // Preimage ImageHashable
        //TODO: If Preimage is set to type IImageHashable, would it be a cyclic definition? Preimage is set to byte[] for now, will modify it accordingly
        public byte[] Preimage { get; set; }

        public static string imageHashApprovalSalt = SequenceCoder.KeccakHash("SetImageHash(bytes32 imageHash)");

        public Digest Approval()
        {           

            return Digest.NewDigest(imageHashApprovalSalt, this.Hash);// Assuming Digest is a valid type and has a constructor accepting the approvalSalt and hashBytes as parameters
        }
    }
}
