using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Provider;
using Sequence.WaaS;
using System;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Transactions;
using Sequence.Utils;

namespace Sequence.WaaS
{
    public class WaaSToWalletAdapter : Sequence.Wallet.IWallet
    {
        private IWallet _wallet;
        private Dictionary<uint, Address> _walletAddressesByAccountIndex;

        public static async Task<WaaSToWalletAdapter> CreateAsync(WaaSWallet wallet)
        {
            var walletAddressesByAccountIndex = new Dictionary<uint, Address>();
            var address = await wallet.GetWalletAddress(null);
            walletAddressesByAccountIndex[0] = new Address(address.address);
            return new WaaSToWalletAdapter(wallet, walletAddressesByAccountIndex);
        }

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

        public async Task<string> SendTransaction(IEthClient client, EthTransaction transaction)
        {
            Transaction waasTransaction = new Transaction((uint)transaction.ChainId.HexStringToInt(), GetAddress(), transaction.To, null, null, transaction.Value.ToString(), transaction.Data);
            SendTransactionArgs args = new SendTransactionArgs(waasTransaction);
            SendTransactionReturn result = await _wallet.SendTransaction(args);
            return result.txHash;
        }

        public async Task<TransactionReceipt> SendTransactionAndWaitForReceipt(IEthClient client, EthTransaction transaction)
        {
            string transactionHash = await SendTransaction(client, transaction);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
            return receipt;
        }

        public async Task<string[]> SendTransactionBatch(IEthClient client, EthTransaction[] transactions)
        {
            int transactionCount = transactions.Length;
            Transaction[] waasTransactions = new Transaction[transactionCount];
            for (int i = 0; i < transactionCount; i++)
            {
                waasTransactions[i] = new Transaction((uint)transactions[i].ChainId.HexStringToInt(), GetAddress(), transactions[i].To, null, null, transactions[i].Value.ToString(), transactions[i].Data);
            }

            SendTransactionBatchArgs args = new SendTransactionBatchArgs(waasTransactions);
            SendTransactionBatchReturn result = await _wallet.SendTransactionBatch(args);
            return new string[]{result.txHash};
        }

        public async Task<TransactionReceipt[]> SendTransactionBatchAndWaitForReceipts(IEthClient client, EthTransaction[] transactions)
        {
            string[] transactionHashes = await SendTransactionBatch(client, transactions);
            int transactionCount = transactionHashes.Length;
            TransactionReceipt[] receipts = new TransactionReceipt[transactionCount];
            for (int i = 0; i < transactionCount; i++)
            {
                receipts[i] = await client.WaitForTransactionReceipt(transactionHashes[i]);
            }

            return receipts;
        }

        public async Task<string> SignMessage(byte[] message, byte[] chainId)
        {
            string messageString = SequenceCoder.HexStringToHumanReadable(SequenceCoder.ByteArrayToHexString(message));
            string chainIdString =
                SequenceCoder.HexStringToHumanReadable(SequenceCoder.ByteArrayToHexString(chainId));

            return await SignMessage(messageString, chainIdString);
        }

        public async Task<string> SignMessage(string message, string chainId)
        {
            SignMessageArgs args = new SignMessageArgs(GetAddress(), chainId, message);
            var result = await _wallet.SignMessage(args);
            return result.signature;
        }

        public async Task<bool> IsValidSignature(string signature, string message, string chainId, uint accountIndex = 0)
        {
            var args = new IsValidMessageSignatureArgs((uint)chainId.HexStringToInt(), GetAddress(accountIndex), message, signature);
            var result = await _wallet.IsValidMessageSignature(args);
            return result.isValid;
        }
    }
}