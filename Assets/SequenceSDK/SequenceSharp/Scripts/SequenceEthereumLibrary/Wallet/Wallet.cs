#define HAS_SPAN
#define SECP256K1_LIB

using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SequenceSharp.RPC;
using SequenceSharp.ABI;
using System.Text;
using NBitcoin.Secp256k1;



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
        public ECPrivKey privateKey;
        public ECPubKey publicKey;
        private static byte[] testingInput = SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");


        public Wallet()
        {
            privateKey = ECPrivKey.Create(testingInput);
            publicKey = privateKey.CreatePubKey();


        }

        public Wallet(string _privateKey)
        {
            privateKey = ECPrivKey.Create(SequenceCoder.HexStringToByteArray(_privateKey));
            publicKey = privateKey.CreatePubKey();

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
            byte[] publickeyBytes = publicKey.ToBytes(false);
            byte[] publicKeyBytes64 = new byte[64];

            Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64);

            string hashed = SequenceCoder.ByteArrayToHexString(SequenceCoder.KeccakHash(publicKeyBytes64));
            int length = hashed.Length;
            string address = hashed.Substring(length - 40);

            address = SequenceCoder.AddressChecksum(address);
            return address;

        }

        public string RecoverAddress()
        {
            throw new System.NotImplementedException();
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

        public BigInteger GetBalance()
        {
            throw new System.NotImplementedException();
        }

        public BigInteger GetNonce()
        {
            throw new System.NotImplementedException();
        }

        public void SignTx()
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidSignature(string publicKey, string sigHash, string hiMessage)
        {
            //ECDSAKey.PublicKey
            bool valid = SequenceCoder.VerifySignatureD(publicKey, sigHash, hiMessage);
            return valid;
        }
        public bool SignMessage(byte[] message, out SecpECDSASignature signature)
        {
            //throw new System.NotImplementedException();
            //TODO: message 191 :?

            byte[] message191 = Encoding.UTF8.GetBytes(@"\x19Ethereum Signed Message:\n");
            //TODO: Check message has message191 has prefix

            // byte[] hash = SequenceCoder.KeccakHash(message);

            //byte[] signatureBytes
            UnityEngine.Debug.Log("message without prefix: " + SequenceCoder.ByteArrayToHexString(message));
            UnityEngine.Debug.Log("message with prefix: " + SequenceCoder.ByteArrayToHexString(message));
            UnityEngine.Debug.Log("length: " + message.Length);

            byte[] message32 = new byte[32];
            int len = message.Length;
            byte[] messageLen = Encoding.UTF8.GetBytes(len.ToString());
            message = (message191.Concat(messageLen).ToArray()).Concat(message).ToArray(); // with prefix
            //message = messageLen.Concat(message).ToArray(); //without prefix
            UnityEngine.Debug.Log("message concatenated before hash: " + SequenceCoder.ByteArrayToHexString(message));
            message32 = SequenceCoder.KeccakHash(message);

            UnityEngine.Debug.Log("message after hash: " + SequenceCoder.ByteArrayToHexString(message32));
            bool signed = privateKey.TrySignECDSA(message32, out signature);//SequenceCoder.SignDataD(message, ECDSAKey.PrivateKey);
            //TODO: ?
            byte[] sigHash64 = new byte[64];

            signature.WriteCompactToSpan(sigHash64);
            
            UnityEngine.Debug.Log("signature hash: " + SequenceCoder.ByteArrayToHexString(sigHash64));
/*
            Span<byte> sigHashSpan = stackalloc byte[75];
            int sigLen = 0;
            signature.WriteDerToSpan(sigHashSpan, out sigLen);
            UnityEngine.Debug.Log("signature hash: " + SequenceCoder.ByteArrayToHexString(sigHashSpan.ToArray()));
            UnityEngine.Debug.Log("signature len: " + sigLen);*/

            

            
            return signed;
        }




    }
}