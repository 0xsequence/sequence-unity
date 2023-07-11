using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Provider;
using UnityEngine;

namespace Sequence.Wallet
{
    public interface IWallet 
    {
        public string GetAddress();
        public Task<BigInteger> GetBalance(IEthClient client);
        public Task<BigInteger> GetNonce(IEthClient client);
        public (string v, string r, string s) SignTransaction(byte[] message, string chainId);
        public Task<string> SendRawTransaction(IEthClient client, string signedTransactionData);
        public Task<TransactionReceipt> SendRawTransactionAndWaitForReceipt(IEthClient client, string signedTransactionData);

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
        public string SignMessage(byte[] message);

        /// <summary>
        /// Signs a message with the wallet's private key.
        /// </summary>
        /// <param name="message">The message to sign as a string.</param>
        /// <returns>The signature as a string.</returns>
        public string SignMessage(string message);

        /// <summary>
        /// Verifies the validity of a signature for a given message.
        /// </summary>
        /// <param name="signature">The signature to verify.</param>
        /// <param name="message">The message that was signed.</param>
        /// <returns><c>true</c> if the signature is valid, <c>false</c> otherwise.</returns>
        public bool IsValidSignature(string signature, string message);

        /// <summary>
        /// Recovers the Ethereum address from a message and its signature.
        /// </summary>
        /// <param name="message">The message that was signed.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns>The Ethereum address as a string.</returns>
        public string Recover(string message, string signature);


        /// <summary>
        /// Adds the Ethereum Signed Message prefix to a message.
        /// </summary>
        /// <param name="message">The message to prefix.</param>
        /// <returns>The prefixed message as a byte array.</returns>
        public static byte[] PrefixedMessage(byte[] message)
        {
            // https://eips.ethereum.org/EIPS/eip-191
            byte[] message191 = SequenceCoder.HexStringToByteArray("19").Concat(Encoding.UTF8.GetBytes("Ethereum Signed Message:\n")).ToArray();
            byte[] messageLen = Encoding.UTF8.GetBytes((message.Length).ToString());
            if (!message.Take(message191.Length).SequenceEqual(message191))
            {
                message = (message191.Concat(messageLen).ToArray()).Concat((message)).ToArray();
            }

            return message;
        }
    }
}
