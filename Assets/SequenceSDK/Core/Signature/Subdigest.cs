using System;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Wallet;

namespace Sequence.Core.Signature
{
    public class Subdigest
    {
        public Hash Hash { get; set; }
        // Digest is the preimage of the subdigest, null if unknown.
        public Digest Digest { get; set; }
        // Wallet is the target wallet of the subdigest, null if unknown.
        public Address WalletAddress { get; set; }
        // ChainID is the target chain ID of the subdigest, null if unknown.
        public BigInteger ChainID { get; set; }
        // EthSignPreimage is the preimage of the eth_sign subdigest, null if unknown.
        public Subdigest EthSignPreimage { get; set; }

        // EthSignSubdigest derives the eth_sign subdigest of a subdigest.
        public Subdigest EthSignSubdigest()
        {
            byte[] messagePrefix = Encoding.UTF8.GetBytes("\x19Ethereum Signed Message:\n");
            byte[] messageLength = Encoding.UTF8.GetBytes(Hash.Bytes.Length.ToString());
            byte[] message = Hash.Bytes;

            byte[] concatenatedBytes = ByteArrayExtensions.ConcatenateByteArrays(messagePrefix, messageLength, message);

            byte[] keccak256Hash = SequenceCoder.KeccakHash(concatenatedBytes);
            return new Subdigest
            {
                Hash = new Hash(keccak256Hash),
                EthSignPreimage = this
            };
        }
    }
}