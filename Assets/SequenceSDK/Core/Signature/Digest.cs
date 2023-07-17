using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Extensions;

namespace Sequence.Core.Signature
{
    public class Digest
    {
        public Hash Hash { get; set; }
        // Preimage is the preimage of the digest
        public byte[] Preimage { get; set; }

        public static Digest NewDigest(params string[] messages)
        {
            byte[] preimage = Encoding.UTF8.GetBytes(string.Join("", messages));
            return new Digest
            { 
                Hash = new Hash(SequenceCoder.KeccakHash(preimage)),
                Preimage = preimage
            };
        }

        public (ImageHash, Exception) ApprovedImageHash()
        {
            byte[] approvalSalt = ImageHash.ApprovalSalt.ToByteArray();
            int approvalSaltLength = approvalSalt.Length;
            if (!Preimage.HasPrefix(approvalSalt) ||
                Preimage.Length != approvalSaltLength + Hash.HashLength)
            {
                return (null, new Exception($"Preimage {Preimage.ByteArrayToHexStringWithPrefix()} of {Hash} is not an image hash approval"));
            }

            byte[] hashBytes = Preimage.AsSpan(approvalSaltLength).ToArray();
            return (new ImageHash()
            {
                Hash = new Hash(hashBytes),
            }, null);;
        }

        // Subdigest derives the hash to be signed by the Sequence wallet's signers to validate the digest.
        public Subdigest Subdigest(Address wallet, params BigInteger[] chainID)
        {
            if (chainID.Length == 0 || chainID[0] == null)
            {
                chainID = new BigInteger[] { BigInteger.Zero };
            }

            if (chainID[0] == null)
            {
                chainID[0] = BigInteger.Zero;
            }

            byte[] chainIDBytes = chainID[0].ToByteArray();
            Array.Reverse(chainIDBytes);

            byte[] data = ByteArrayExtensions.ConcatenateByteArrays(
                new byte[] { 0x19, 0x01 },
                chainIDBytes,
                wallet.Value.ToByteArray(),
                Hash);


            return new Subdigest
            {
                Hash = new Hash(SequenceCoder.KeccakHash(data)),
                Digest = this,
                WalletAddress = new Address(wallet),
                ChainID = chainID[0]
            };
        }
    }
}