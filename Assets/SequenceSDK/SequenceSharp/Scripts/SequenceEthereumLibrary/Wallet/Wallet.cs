using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SequenceSharp.RPC;
using SequenceSharp.ABI;
using System.Text;

namespace SequenceSharp.WALLET
{

    public struct WalletOptions
    {
        string derivationPath;
        int randomWalletEntropyBitSize;
    }
    public class Wallet : BaseWallet
    {
        Provider provider;
        WalletProvider walletProvider;
        public ECDSAKey key;
        //Only For Testing: (TODO)
        public string publicKey;
        public string privateKey;
        public Wallet()
        {
            key = new ECDSAKey();
            
        }

       public Wallet(string _privateKey)
        {
            privateKey = _privateKey;
            publicKey = ECDSAKey.PublicKeyFromPrivateKey(_privateKey);
            
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
            string hashedPublic = SequenceCoder.KeccakHash(publicKey);
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
        public override byte[] SignMessage(byte[] message)
        {
            //throw new System.NotImplementedException();
            //TODO: message 191 :?
            byte[] message191 = Encoding.UTF8.GetBytes("\x19Ethereum Signed Message:\n");
            //TODO: Check message has message191 has prefix

            byte[] hash = SequenceCoder.KeccakHash(message);

            byte[] signature = SequenceCoder.SignDataD(message, ECDSAKey.PrivateKey);
            //TODO: ?

            //signature[64] += 27;
            return signature;
        }

        public override void SendTransaction()
        {
            throw new System.NotImplementedException();
        }

        public override void SignTypedData()
        {
            throw new System.NotImplementedException();
        }

        
        
    }
}