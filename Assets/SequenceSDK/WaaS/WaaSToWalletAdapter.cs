using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Provider;
using Sequence.WaaS;
using System;
using Sequence.ABI;
using Sequence.Extensions;

namespace Sequence.WaaS
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

        public async Task<string> SignMessage(byte[] message, byte[] chainId = null)
        {
            string messageString = SequenceCoder.HexStringToHumanReadable(SequenceCoder.ByteArrayToHexString(message));
            string chainIdString =
                SequenceCoder.HexStringToHumanReadable(SequenceCoder.ByteArrayToHexString(chainId));

            return await SignMessage(messageString, chainIdString);
        }

        public async Task<string> SignMessage(string message, string chainId = null)
        {
            if (uint.TryParse(chainId, out uint chain))
            {
                SignMessageArgs args = new SignMessageArgs(chain, GetAddress(), message);
                var result = await _wallet.SignMessage(args);
                return result.signature;
            }
            else
            {
                throw new ArgumentException($"{nameof(chainId)} must be parseable to an {typeof(uint)}, given: {chainId}");
            }
        }

        public async Task<bool> IsValidSignature(string signature, string message, uint accountIndex, string chainId)
        {
            var args = new IsValidMessageSignatureArgs(chainId, GetAddress(accountIndex), message, signature);
            var result = await _wallet.IsValidMessageSignature(args);
            return result.isValid;
        }
    }
}