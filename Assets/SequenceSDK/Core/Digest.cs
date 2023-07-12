using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Sequence.ABI;

namespace Sequence.Core {
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