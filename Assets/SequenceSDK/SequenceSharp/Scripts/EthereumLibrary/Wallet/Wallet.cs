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
        ECDSAKey key = new ECDSAKey();
        
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

        public bool IsValidSignature()
        {
            throw new System.NotImplementedException();
        }
        public override byte[] SignMessage(byte[] message)
        {
            //throw new System.NotImplementedException();
            //TODO: message 191 :?
            byte[] message191 = Encoding.UTF8.GetBytes("\x19Ethereum Signed Message:\n");
            //TODO: Check message has message191 has prefix

            byte[] hash = SequenceCoder.KeccakHash(message191);

            byte[] signature = SequenceCoder.SignData(hash, key.PrivateKey);
            //TODO: ?

            signature[64] += 27;
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