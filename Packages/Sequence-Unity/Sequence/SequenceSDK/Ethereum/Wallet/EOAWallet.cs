#define HAS_SPAN
#define SECP256K1_LIB

using NBitcoin.Secp256k1;
using Sequence.Signer;
using Sequence.Provider;
using Sequence.ABI;
using System.Text;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.Transactions;

namespace Sequence.Wallet
{
    public class EOAWallet : IWallet
    {
        public ECPrivKey privKey;
        public ECPubKey pubKey;
        private Address address;

        /// <summary>
        /// Initializes a new instance of the <see cref="EOAWallet"/> class with a randomly generated private key.
        /// </summary>
        public EOAWallet()
        {
            byte[] seed = Org.BouncyCastle.Security.SecureRandom.GetInstance("SHA256PRNG").GenerateSeed(64);
            privKey = ECPrivKey.Create(seed);
            pubKey = privKey.CreatePubKey();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EOAWallet"/> class with the specified private key.
        /// </summary>
        /// <param name="_privateKey">The private key as a hexadecimal string.</param>
        public EOAWallet(string _privateKey)
        {
            privKey = ECPrivKey.Create(SequenceCoder.HexStringToByteArray(_privateKey));
            pubKey = privKey.CreatePubKey();
        }

        /// <summary>
        /// Generates the Ethereum address associated with the wallet.
        /// </summary>
        /// <returns>The Ethereum address as a string.</returns>
        private string GenerateAddress()
        {
            //Last 20 bytes of the Keccak-256 hash of the public key
            byte[] publickeyBytes = pubKey.ToBytes(false);
            byte[] publicKeyBytes64 = new byte[64];

            Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64);

            return IWallet.PubkeyToAddress(publicKeyBytes64);
        }

        public Address GetAddress()
        {
            if (address == null)
            {
                address = new Address(GenerateAddress());
            }
            return address;
        }

        public async Task<TransactionReceipt> DeployContract(IEthClient client, string bytecode, ulong value = 0)
        {
            EthTransaction deployTransaction = await new GasLimitEstimator(client, GetAddress()).BuildTransaction(StringExtensions.ZeroAddress, bytecode, value);
            TransactionReceipt receipt = await SendTransactionAndWaitForReceipt(client, deployTransaction);
            return receipt;

        }
        
        /// <summary>
        /// Get the balance of native currency (e.g. ETH) held by this wallet
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<BigInteger> GetBalance(IEthClient client)
        {
            BigInteger balance = await client.BalanceAt(GetAddress(), "latest");
            return balance;
        }

        /// <summary>
        /// Get the current nonce (as BigInteger) for this wallet
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<BigInteger> GetNonce(IEthClient client)
        {
            return await client.NonceAt(GetAddress());
        }

        public Task<string> SendTransaction(IEthClient client, EthTransaction transaction)
        {
            string signedTransaction = transaction.SignAndEncodeTransaction(this);
            return SendRawTransaction(client, signedTransaction);
        }

        public async Task<TransactionReceipt> SendTransactionAndWaitForReceipt(IEthClient client, EthTransaction transaction)
        {
            string result = await SendTransaction(client, transaction);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(result);
            return receipt;
        }

        public async Task<string[]> SendTransactionBatch(IEthClient client, EthTransaction[] transactions)
        {
            int transactionCount = transactions.Length;
            string[] transactionHashes = new string[transactionCount];
            for (int i = 0; i < transactionCount; i++)
            {
                transactions[i].IncrementNonceBy(i);
                transactionHashes[i] = await SendTransaction(client, transactions[i]);
            }
            
            return transactionHashes;
        }

        public async Task<TransactionReceipt[]> SendTransactionBatchAndWaitForReceipts(IEthClient client, EthTransaction[] transactions)
        {
            string[] transactionHashes = await SendTransactionBatch(client, transactions);
            int transactionCount = transactions.Length;
            if (transactionCount != transactionHashes.Length)
            {
                throw new SystemException("Invalid system state: didn't receive as many transaction hashes as transactions");
            }

            TransactionReceipt[] receipts = new TransactionReceipt[transactionCount];
            for (int i = 0; i < transactionCount; i++)
            {
                receipts[i] = await client.WaitForTransactionReceipt(transactionHashes[i]);
            }

            return receipts;
        }

        public (string v, string r, string s) SignTransaction(byte[] message, string chainId)
        {
            int id = chainId.HexStringToInt();
            return EthSignature.SignAndReturnVRS(message, privKey, id);
        }

        private async Task<string> SendRawTransaction(IEthClient client, string signedTransactionData)
        {
            string result = await client.SendRawTransaction(signedTransactionData);
            return result;
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
        public async Task<string> SignMessage(byte[] message, byte[] chainId = null)
        {
            if (chainId != null && chainId.Length > 0)
            {
                message = ByteArrayExtensions.ConcatenateByteArrays(message, chainId);
            }
            byte[] message32 = SequenceCoder.KeccakHash(PrefixedMessage(message));
            return EthSignature.Sign(message32, privKey);
        }


        /// <summary>
        /// Signs a message with the wallet's private key.
        /// </summary>
        /// <param name="message">The message to sign as a string.</param>
        /// <returns>The signature as a string.</returns>
        public Task<string> SignMessage(string message, string chainId = null)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] chainIdBytes = null;
            if (chainId != null)
            {
                chainIdBytes = chainId.HexStringToByteArray();
            }
            return SignMessage(messageBytes, chainIdBytes);
        }

        /// <summary>
        /// Signs a message with a specific private key.
        /// </summary>
        /// <param name="privateKey">The private key as a hexadecimal string.</param>
        /// <param name="message">The message to sign as a string.</param>
        /// <returns>The signature as a string.</returns>
        public string SignMessageWithPrivateKey(string privateKey, string message)
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
        public async Task<bool> IsValidSignature(string signature, string message, Chain chain = Chain.None)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            if (chain != Chain.None)
            {
                messageBytes = ByteArrayExtensions.ConcatenateByteArrays(messageBytes, chain.AsHexString().HexStringToByteArray());
            }
            byte[] messagePrefix = PrefixedMessage(messageBytes);
            byte[] hashedMessage = SequenceCoder.KeccakHash(messagePrefix);
            SecpRecoverableECDSASignature recoverable = EthSignature.GetSignature(signature);

            if (recoverable != null)
            {
                SecpECDSASignature sig = recoverable.ToSignature();

                return pubKey.SigVerify(sig, hashedMessage);
            }

            return false;
        }

        /// <summary>
        /// Adds the Ethereum Signed Message prefix to a message.
        /// </summary>
        /// <param name="message">The message to prefix.</param>
        /// <returns>The prefixed message as a byte array.</returns>
        public static byte[] PrefixedMessage(byte[] message)
        {
            return IWallet.PrefixedMessage(message);
        }

        /// <summary>
        /// Recovers the Ethereum address from a message and its signature.
        /// </summary>
        /// <param name="message">The message that was signed.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns>The Ethereum address as a string.</returns>
        public static string Recover(string message, string signature)
        {
            return IWallet.Recover(message, signature);
        }
    }
}

