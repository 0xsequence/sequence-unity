#define HAS_SPAN
#define SECP256K1_LIB


using NBitcoin.Secp256k1;
using Sequence.Signer;
using Sequence.Provider;
using Sequence.ABI;
using System.Text;
using System.Linq;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.Wallet
{
    public class EthWallet
    {
        public ECPrivKey privKey;
        public ECPubKey pubKey;
        private string address;

        /// <summary>
        /// Initializes a new instance of the <see cref="EthWallet"/> class with a randomly generated private key.
        /// </summary>
        public EthWallet()
        {
            byte[] seed = Org.BouncyCastle.Security.SecureRandom.GetInstance("SHA256PRNG").GenerateSeed(64);
            privKey = ECPrivKey.Create(seed);
            pubKey = privKey.CreatePubKey();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EthWallet"/> class with the specified private key.
        /// </summary>
        /// <param name="_privateKey">The private key as a hexadecimal string.</param>
        public EthWallet(string _privateKey)
        {
            privKey = ECPrivKey.Create(SequenceCoder.HexStringToByteArray(_privateKey));
            pubKey = privKey.CreatePubKey();
        }

        /// <summary>
        /// Retrieves the Ethereum address associated with the wallet.
        /// </summary>
        /// <returns>The Ethereum address as a string.</returns>
        public string GenerateAddress()
        {
            //Last 20 bytes of the Keccak-256 hash of the public key
            byte[] publickeyBytes = pubKey.ToBytes(false);
            byte[] publicKeyBytes64 = new byte[64];

            Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64);

            return PubkeyToAddress(publicKeyBytes64);
        }

        public string GetAddress()
        {
            if (address == null)
            {
                address = GenerateAddress();
            }
            return address;
        }

        public async Task<BigInteger> GetBalance(SequenceEthClient client)
        {
            string blockNumber = await client.BlockNumber();
            BigInteger balance = await client.BalanceAt(GetAddress(), blockNumber);
            return balance;
        }

        public async Task<BigInteger> GetNonce(IEthClient client)
        {
            return await client.NonceAt(GetAddress());
        }

        public (string v, string r, string s) SignTransaction(byte[] message, int chainId)
        {
            return EthSignature.SignAndReturnVRS(message, privKey, chainId);
        }

        public (string v, string r, string s) SignTransaction(byte[] message)
        {
            return EthSignature.SignAndReturnVRS(message, privKey);
        }

        public async Task<string> SendRawTransaction(IEthClient client, string signedTransactionData)
        {
            string result = await client.SendRawTransaction(signedTransactionData);
            return result;
        }

        public async Task<TransactionReceipt> SendRawTransactionAndWaitForReceipt(IEthClient client, string signedTransactionData)
        {
            string result = await SendRawTransaction(client, signedTransactionData);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(result);
            return receipt;
        }

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
        public string SignMessage(byte[] message)
        {
            byte[] message32 = SequenceCoder.KeccakHash(PrefixedMessage(message));
            return EthSignature.Sign(message32, privKey);
        }


        /// <summary>
        /// Signs a message with the wallet's private key.
        /// </summary>
        /// <param name="message">The message to sign as a string.</param>
        /// <returns>The signature as a string.</returns>
        public string SignMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            return SignMessage(messageBytes);
        }

        /// <summary>
        /// Signs a message with a specific private key.
        /// </summary>
        /// <param name="privateKey">The private key as a hexadecimal string.</param>
        /// <param name="message">The message to sign as a string.</param>
        /// <returns>The signature as a string.</returns>
        public string SignMessage(string privateKey, string message)
        {
            byte[] message32 = new byte[32];
            message32 = SequenceCoder.KeccakHash(PrefixedMessage(Encoding.UTF8.GetBytes(message)));

            ECPrivKey privKey = Context.Instance.CreateECPrivKey(SequenceCoder.HexStringToByteArray(privateKey));
            return EthSignature.Sign(message32, privKey);
        }

        /// <summary>
        /// Signs a byte array with a specific private key.
        /// </summary>
        /// <param name="privateKey">The private key as a hexadecimal string.</param>
        /// <param name="byteArray">The byte array to sign.</param>
        /// <returns>The signature as a string.</returns>
        public string SignByteArray(string privateKey, byte[] byteArray)
        {
            byte[] prefixed = new byte[32];
            prefixed = SequenceCoder.KeccakHash(PrefixedMessage(byteArray));
            ECPrivKey privKey = Context.Instance.CreateECPrivKey(SequenceCoder.HexStringToByteArray(privateKey));
            return EthSignature.Sign(prefixed, privKey);
        }


        /// <summary>
        /// Verifies the validity of a signature for a given message.
        /// </summary>
        /// <param name="signature">The signature to verify.</param>
        /// <param name="message">The message that was signed.</param>
        /// <returns><c>true</c> if the signature is valid, <c>false</c> otherwise.</returns>
        public bool IsValidSignature(string signature, string message)
        {
            byte[] messagePrefix = PrefixedMessage(Encoding.UTF8.GetBytes(message));
            byte[] hashedMessage = SequenceCoder.KeccakHash(messagePrefix);
            SecpRecoverableECDSASignature recoverble = EthSignature.GetSignature(signature);

            if (recoverble != null)
            {
                SecpECDSASignature sig = recoverble.ToSignature();

                return pubKey.SigVerify(sig, hashedMessage);
            }

            return false;
        }

        /// <summary>
        /// Recovers the Ethereum address from a message and its signature.
        /// </summary>
        /// <param name="message">The message that was signed.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns>The Ethereum address as a string.</returns>
        public string Recover(string message, string signature)
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

        /// <summary>
        /// Converts a public key to an Ethereum address.
        /// </summary>
        /// <param name="pubkey">The public key byte array.</param>
        /// <returns>The Ethereum address derived from the public key.</returns>
        private string PubkeyToAddress(byte[] pubkey)
        {
            string hashed = SequenceCoder.ByteArrayToHexString(SequenceCoder.KeccakHash(pubkey));
            int length = hashed.Length;
            string address = hashed.Substring(length - 40);

            address = SequenceCoder.AddressChecksum(address);
            return address;
        }
    }
}

