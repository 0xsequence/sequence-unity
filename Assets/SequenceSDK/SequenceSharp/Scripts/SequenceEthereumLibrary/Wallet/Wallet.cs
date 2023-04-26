#define HAS_SPAN
#define SECP256K1_LIB

using System;
using System.Linq;
//using System.Numerics;
using System.Threading.Tasks;
using SequenceSharp.RPC;
using SequenceSharp.ABI;
using System.Text;
using NBitcoin.Secp256k1;
using SequenceSharp.Signer;
using System.Collections.Generic;

namespace SequenceSharp.WALLET
{

    public struct WalletOptions
    {
        string derivationPath;
        int randomWalletEntropyBitSize;
    }
    public class Wallet
    {
        Provider provider;
        WalletProvider walletProvider;

        //Switch to NBitcoin.Secp256k1 functions
        //TODO: Testing, will refactor later
        public ECPrivKey privKey;
        public ECPubKey pubKey;


        public Wallet()
        {
            //TODO: ...
            byte[] seed = Org.BouncyCastle.Security.SecureRandom.GetInstance("SHA256PRNG").GenerateSeed(64);
            privKey = ECPrivKey.Create(seed);
            pubKey = privKey.CreatePubKey();

        }

        public Wallet(string _privateKey)
        {
            privKey = ECPrivKey.Create(SequenceCoder.HexStringToByteArray(_privateKey));
            pubKey = privKey.CreatePubKey();

        }

        public Task Encrypt()
        {
            throw new System.NotImplementedException();
        }

        public void EncryptSync()
        {
            throw new System.NotImplementedException();
        }

        public static Task<Wallet> FromEncryptedJson()
        {
            throw new System.NotImplementedException();
        }

        public static Wallet FromEncryptedJsonSync()
        {
            throw new System.NotImplementedException();
        }
        /*
                public static HDWallet CreatRandom()
                {
                    throw new System.NotImplementedException();
                }

                public static HDWallet FromPrase()
                {
                    throw new System.NotImplementedException();
                }
        */

        public string Address()
        {
            //TODO: Address return type 

            //Last 20 bytes of the Keccak-256 hash of the public key
            byte[] publickeyBytes = pubKey.ToBytes(false);
            byte[] publicKeyBytes64 = new byte[64];

            Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64);

            return PubkeyToAddress(publicKeyBytes64);

        }

        

        public string RecoverAddressFromDigest()
        {
            throw new System.NotImplementedException();
        }

        public void Transactor(Wallet wallet)
        {

            throw new System.NotImplementedException();
        }

        public void TransactorForChainID(Wallet wallet)
        {
            throw new System.NotImplementedException();
        }

        public void GetProvider()
        {
            throw new System.NotImplementedException();
        }

        public void SetProvider(Provider _provider)
        {
            provider = _provider;

            //throw new System.NotImplementedException();
        }
        public void Provider()
        {
            throw new System.NotImplementedException();
        }

        public System.Numerics.BigInteger GetBalance()
        {
            throw new System.NotImplementedException();
        }

        public System.Numerics.BigInteger GetNonce()
        {
            throw new System.NotImplementedException();
        }

        public void SignTx()
        {
            throw new System.NotImplementedException();
        }

        public (string v, string r, string s) SignTx(byte[] message, int chainId)
        {
            return EthSignature.SignAndReturnVRS(message, privKey, chainId);
        }

        public (string v, string r, string s) SignTx(byte[] message)
        {
            return EthSignature.SignAndReturnVRS(message, privKey);
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
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public string SignMessage(byte[] message)
        {                       
            byte[] message32 = SequenceCoder.KeccakHash(prefixedMessage(message));
            return EthSignature.Sign(message32, privKey);
        }



        public string SignMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            return SignMessage(messageBytes);
        }

        public string SignMessage(string privateKey, string message)
        {
         
            byte[] message32 = new byte[32];
            message32 = SequenceCoder.KeccakHash(prefixedMessage(Encoding.UTF8.GetBytes(message)));

            ECPrivKey privKey = Context.Instance.CreateECPrivKey(SequenceCoder.HexStringToByteArray(privateKey));
            return EthSignature.Sign(message32, privKey);
        
        }

        public string SignByteArray(string privateKey, byte[] byteArray)
        {
            byte[] prefixed = new byte[32];
            prefixed = SequenceCoder.KeccakHash(prefixedMessage(byteArray));
            ECPrivKey privKey = Context.Instance.CreateECPrivKey(SequenceCoder.HexStringToByteArray(privateKey));
            return EthSignature.Sign(prefixed, privKey);
        }


        public bool IsValidSignature(string signature, string message)
        {
            byte[] messagePrefix = prefixedMessage(Encoding.UTF8.GetBytes(message));
            byte[] hashedMessage = SequenceCoder.KeccakHash(messagePrefix);
            SecpRecoverableECDSASignature recoverble = EthSignature.GetSignature(signature);

            if (recoverble != null)
            {
                SecpECDSASignature sig = recoverble.ToSignature();

                return pubKey.SigVerify(sig, hashedMessage);
            }


            return false;


        }


        public string Recover(string message, string signature)
        {
            byte[] messagePrefix = prefixedMessage(Encoding.UTF8.GetBytes(message));
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
        /// https://eips.ethereum.org/EIPS/eip-191
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] prefixedMessage(byte[] message)
        {

            byte[] message191 = SequenceCoder.HexStringToByteArray("19").Concat(Encoding.UTF8.GetBytes("Ethereum Signed Message:\n")).ToArray();
            byte[] messageLen = Encoding.UTF8.GetBytes((message.Length).ToString());
            if (!message.Take(message191.Length).SequenceEqual(message191))
            {
                
                message = (message191.Concat(messageLen).ToArray()).Concat((message)).ToArray();
                
            }
            
            return message;
        }


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