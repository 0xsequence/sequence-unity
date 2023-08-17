using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NBitcoin.Secp256k1;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Provider;
using Sequence.Signer;
using Sequence.Transactions;
using SequenceSDK.Ethereum.Utils;
using UnityEngine;

namespace Sequence.Wallet
{
    public interface IWallet 
    {
        public Address GetAddress(uint accountIndex = 0);
        public Task<string> SendTransaction(IEthClient client, EthTransaction transaction);
        public Task<TransactionReceipt> SendTransactionAndWaitForReceipt(IEthClient client, EthTransaction transaction);
        public Task<string[]> SendTransactionBatch(IEthClient client, EthTransaction[] transactions);

        public Task<TransactionReceipt[]> SendTransactionBatchAndWaitForReceipts(IEthClient client,
            EthTransaction[] transactions);

        /// <summary>
        /// 
        /// https://docs.ethers.org/v5/api/signer/#Signer-signMessage
        /// If message is a string, it is treated as a string and converted to its representation in UTF8 bytes.

        ///If and only if a message is a Bytes will it be treated as binary data.

        ///For example, the string "0x1234" is 6 characters long (and in this case 6 bytes long). This is not equivalent to the array[0x12, 0x34], which is 2 bytes long.

        ///A common case is to sign a hash.In this case, if the hash is a string, it must be converted to an array first, using the arrayify utility function.
        /// 
        /// 
        /// </summary>
        /// <param name="message">The message to sign as a byte array.</param>
        /// <returns>The signature as a string.</returns>
        public Task<string> SignMessage(byte[] message, byte[] chainId = null);

        /// <summary>
        /// Signs a message with the wallet's private key.
        /// </summary>
        /// <param name="message">The message to sign as a string.</param>
        /// <returns>The signature as a string.</returns>
        public Task<string> SignMessage(string message, string chainId = null);

        /// <summary>
        /// Verifies the validity of a signature for a given message.
        /// </summary>
        /// <param name="signature">The signature to verify.</param>
        /// <param name="message">The message that was signed.</param>
        /// <param name="accountIndex"></param>
        /// <param name="chainId"></param>
        /// <returns><c>true</c> if the signature is valid, <c>false</c> otherwise.</returns>
        public Task<bool> IsValidSignature(string signature, string message, uint accountIndex = 0, string chainId = "");

        

        /// <summary>
        /// Recovers the Ethereum address from a message and its signature.
        /// </summary>
        /// <param name="message">The message that was signed.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns>The Ethereum address as a string.</returns>
        public static string Recover(string message, string signature)
        {
            byte[] messagePrefix = PrefixedMessage(Encoding.UTF8.GetBytes(message));
            byte[] hashedMessage = SequenceCoder.KeccakHash(messagePrefix);

            SecpRecoverableECDSASignature recoverble = EthSignature.GetSignature(signature);
            ECPubKey _pubkey;
            var ctx = Context.Instance;
            ECPubKey.TryRecover(ctx, recoverble, hashedMessage, out _pubkey);

            byte[] publickeyBytes = _pubkey.ToBytes(false);
            byte[] publicKeyBytes64 = new byte[64];
            Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64); //trim extra 0 at the beginning...

            return PubkeyToAddress(publicKeyBytes64);
        }

        /// <summary>
        /// Converts a public key to an Ethereum address.
        /// </summary>
        /// <param name="pubkey">The public key byte array.</param>
        /// <returns>The Ethereum address derived from the public key.</returns>
        internal static string PubkeyToAddress(byte[] pubkey)
        {
            string hashed = SequenceCoder.ByteArrayToHexString(SequenceCoder.KeccakHash(pubkey));
            int length = hashed.Length;
            string address = hashed.Substring(length - 40);

            address = SequenceCoder.AddressChecksum(address);
            return address;
        }

        /// <summary>
        /// Adds the Ethereum Signed Message prefix to a message.
        /// </summary>
        /// <param name="message">The message to prefix.</param>
        /// <returns>The prefixed message as a byte array.</returns>
        public static byte[] PrefixedMessage(byte[] message)
        {
            // https://eips.ethereum.org/EIPS/eip-191
            byte[] message191 = ByteArrayExtensions.ConcatenateByteArrays(SequenceCoder.HexStringToByteArray("19"), Encoding.UTF8.GetBytes("Ethereum Signed Message:\n"));
            byte[] messageLen = Encoding.UTF8.GetBytes((message.Length).ToString());
            if (!message.Take(message191.Length).SequenceEqual(message191))
            {
                message = ByteArrayExtensions.ConcatenateByteArrays(message191, messageLen, message);
            }

            return message;
        }
    }
}
