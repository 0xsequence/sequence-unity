using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.Transactions;
using UnityEngine;
using static Sequence.Contracts.Contract;

namespace Sequence.Contracts
{
    public static class ContractTransactionSender
    {
        public static async Task<string> SendTransactionMethod(
            this Contract contract,
            IWallet wallet,
            IEthClient client,
            BigInteger value,
            string functionName,
            params object[] functionArgs)
        {
            ContractCall callingInfo = new ContractCall(wallet.GetAddress(), value);
            EthTransaction transaction = await contract.CallFunction(functionName, functionArgs)(client, callingInfo);
            string signedTransaction = transaction.SignAndEncodeTransaction(wallet);
            string result = await wallet.SendRawTransaction(client, signedTransaction);
            return result;
        }

        public static async Task<TransactionReceipt> SendTransactionMethodAndWaitForReceipt(
            this Contract contract,
            IWallet wallet,
            IEthClient client,
            BigInteger value,
            string functionName,
            params object[] functionArgs)
        {
            string transactionHash = await contract.SendTransactionMethod(wallet, client, value, functionName, functionArgs);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
            return receipt;
        }

        public static async Task<string> SendTransactionMethod(
            this CallContractFunctionTransactionCreator transactionCreator,
            IWallet wallet,
            IEthClient client,
            BigInteger? value = null)
        {
            EthTransaction transaction = await transactionCreator(client, new ContractCall(wallet.GetAddress(), value));
            string signedTransaction = transaction.SignAndEncodeTransaction(wallet);
            string result = await wallet.SendRawTransaction(client, signedTransaction);
            return result;
        }

        public static async Task<TransactionReceipt> SendTransactionMethodAndWaitForReceipt(
            this CallContractFunctionTransactionCreator transactionCreator,
            IWallet wallet,
            IEthClient client,
            BigInteger? value = null)
        {
            string transactionHash = await SendTransactionMethod(transactionCreator, wallet, client, value);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
            return receipt;
        }
    }
}
