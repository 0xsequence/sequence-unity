using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Provider;
using Sequence.WaaS;

namespace SequenceSDK.WaaS
{
    public class WaaSToWalletAdapter : Sequence.Wallet.IWallet
    {
        private IWallet _wallet;
        private Dictionary<uint, Address> _walletAddressesByAccountIndex;

        private WaaSToWalletAdapter(IWallet wallet, Dictionary<uint, Address> walletAddressesByAccountIndex)
        {
            _wallet = wallet;
            _walletAddressesByAccountIndex = walletAddressesByAccountIndex;
        }

        public static async Task<WaaSToWalletAdapter> CreateAsync(IWallet wallet, uint[] accountIndexes)
        {
            var walletAddressesByAccountIndex = new Dictionary<uint, Address>();
            int accounts = accountIndexes.Length;

            for (int i = 0; i < accounts; i++)
            {
                var addressReturn =
                    await wallet.GetWalletAddress(new GetWalletAddressArgs(accountIndexes[i]));
                walletAddressesByAccountIndex[accountIndexes[i]] = new Address(addressReturn.address);
            }

            return new WaaSToWalletAdapter(wallet, walletAddressesByAccountIndex);
        }
        
        public Address GetAddress(uint accountIndex = 0)
        {
            return _walletAddressesByAccountIndex[accountIndex];
        }

        public Task<BigInteger> GetBalance(IEthClient client)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> GetNonce(IEthClient client)
        {
            throw new System.NotImplementedException();
        }

        public (string v, string r, string s) SignTransaction(byte[] message, string chainId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> SendRawTransaction(IEthClient client, string signedTransactionData)
        {
            throw new System.NotImplementedException();
        }

        public Task<TransactionReceipt> SendRawTransactionAndWaitForReceipt(IEthClient client, string signedTransactionData)
        {
            throw new System.NotImplementedException();
        }

        public string SignMessage(byte[] message)
        {
            throw new System.NotImplementedException();
        }

        public string SignMessage(string message)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidSignature(string signature, string message)
        {
            throw new System.NotImplementedException();
        }

        public string Recover(string message, string signature)
        {
            throw new System.NotImplementedException();
        }
    }
}