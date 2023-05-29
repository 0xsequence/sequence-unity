using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence.ABI;
using Sequence.Wallet;
using System.Text;
using System;
using System.Linq;

namespace Sequence
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

    public class Subdigest
    {
        public string Hash { get; set; }
        // Digest is the preimage of the subdigest
        public Digest Digest { get; set; }
        // Wallet is the target wallet of the subdigest, *common.Address in go-sequence
        public string WalletAddress { get; set; }
        // ChainID is the target chain ID of the subdigest
        public BigInteger ChainID { get; set; }
        // EthSignPreimage is the preimage of the eth_sign subdigest
        public Subdigest EthSignPreimage { get; set; }

        // EthSignSubdigest derives the eth_sign subdigest of a subdigest.
        public Subdigest EthSignSubdigest()
        {
            return new Subdigest
            {
                //TODO : 
                Hash = SequenceCoder.ByteArrayToHexString(EthWallet.PrefixedMessage(Encoding.UTF8.GetBytes(this.Hash))),
                EthSignPreimage = this

            };
        }
    }

    public class Digest
    {
        public byte[] Hash { get; set; }
        // Preimage is the preimage of the digest
        public byte[] Preimage { get; set; }

        public static Digest NewDigest(params string[] messages)
        {
            byte[] preimage = Encoding.UTF8.GetBytes(string.Join("", messages));
            return new Digest
            { 
                Hash = SequenceCoder.KeccakHash(preimage),
                Preimage = preimage
            };
        }

        // Subdigest derives the hash to be signed by the Sequence wallet's signers to validate the digest.
        public Subdigest Subdigest(string walletAddress, params BigInteger[] chainID)
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

            byte[] data = new byte[] { 0x19, 0x01 }
                .Concat(chainIDBytes)
                .Concat(Encoding.UTF8.GetBytes(walletAddress))
                .Concat(this.Hash)
                .ToArray();


            return new Subdigest
            {
                Hash = SequenceCoder.ByteArrayToHexString(SequenceCoder.KeccakHash(data)),
                Digest = this,
                WalletAddress = walletAddress,
                ChainID = chainID[0]
            };
        }


    }


}
