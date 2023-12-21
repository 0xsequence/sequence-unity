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
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public class WaaSToWalletAdapter : Sequence.Wallet.IWallet
    {
        private IWallet _wallet;

        public WaaSToWalletAdapter(IWallet wallet)
        {
            _wallet = wallet;
        }
        
        public Address GetAddress()
        {
            return _wallet.GetWalletAddress();
        }

        public async Task<TransactionReceipt> DeployContract(IEthClient client, string bytecode, ulong value = 0)
        {
            string chainId = await client.ChainID();
            Chain network = chainId.ChainFromHexString();
            ContractDeploymentReturn result = await _wallet.DeployContract(network, bytecode, value.ToString());
            if (result is FailedContractDeploymentReturn failedResult)
            {
                throw new Exception("Failed to deploy contract: " + failedResult.Error);
            }
            else if (result is SuccessfulContractDeploymentReturn successfulResult)
            {
                string transactionHash = successfulResult.TransactionReturn.txHash;
                TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
                if (receipt.contractAddress == null)
                {
                    receipt.contractAddress = successfulResult.DeployedContractAddress.Value;
                }

                return receipt;
            }
            else
            {
                throw new Exception($"Unknown contract deployment result type. Given {result.GetType().Name}");
            }
        }

        public async Task<string> SendTransaction(IEthClient client, EthTransaction transaction)
        {
            RawTransaction waasTransaction = new RawTransaction(transaction.To, transaction.Value.ToString(), transaction.Data);
            SendTransactionArgs args = await BuildTransactionArgs(client, new Transaction[] { waasTransaction });
            TransactionReturn result = await _wallet.SendTransaction(args);
            if (result is FailedTransactionReturn failedResult)
            {
                throw new Exception(failedResult.error);
            }
            else if (result is SuccessfulTransactionReturn successfulResult)
            {
                return successfulResult.txHash;
            }
            else
            {
                throw new Exception($"Unknown transaction result type. Given {result.GetType().Name}");
            }
        }

        private async Task<SendTransactionArgs> BuildTransactionArgs(IEthClient client, Transaction[] transactions)
        {
            string networkId = await client.ChainID();
            SendTransactionArgs args = new SendTransactionArgs(GetAddress(), networkId, transactions);
            return args;
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
            RawTransaction[] waasTransactions = new RawTransaction[transactionCount];
            for (int i = 0; i < transactionCount; i++)
            {
                waasTransactions[i] = new RawTransaction(transactions[i].To, transactions[i].Value.ToString(), transactions[i].Data);
            }

            SendTransactionArgs args = await BuildTransactionArgs(client, waasTransactions);
            TransactionReturn result = await _wallet.SendTransaction(args);
            if (result is FailedTransactionReturn failedResult)
            {
                throw new Exception(failedResult.error);
            }
            else if (result is SuccessfulTransactionReturn successfulResult)
            {
                return new[] { successfulResult.txHash };
            }
            else
            {
                throw new Exception($"Unknown transaction result type. Given {result.GetType().Name}");
            }
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

        public async Task<bool> IsValidSignature(string signature, string message, Chain chain)
        {
            var args = new IsValidMessageSignatureArgs(chain, GetAddress(), message, signature);
            var result = await _wallet.IsValidMessageSignature(args);
            return result.isValid;
        }
    }
}