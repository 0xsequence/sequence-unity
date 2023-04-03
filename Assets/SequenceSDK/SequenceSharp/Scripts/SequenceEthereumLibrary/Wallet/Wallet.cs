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
            string hashedPublic = "";// SequenceCoder.KeccakHash(publicKey.ToString());
            int length = hashedPublic.Length;
            string address = hashedPublic.Substring(length - 40);
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
            //  byte[] message191 = Encoding.UTF8.GetBytes("\x19Ethereum Signed Message:\n");
            //TODO: Check message has message191 has prefix

            // byte[] hash = SequenceCoder.KeccakHash(message);

            //byte[] signatureBytes
            
            bool signed = privateKey.TrySignECDSA(message, out signature);//SequenceCoder.SignDataD(message, ECDSAKey.PrivateKey);
            //TODO: ?
            UnityEngine.Debug.Log(signed);
            //signature[64] += 27;
            return signed;
        }



       
    }
}