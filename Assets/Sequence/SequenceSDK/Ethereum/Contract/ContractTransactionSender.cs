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
            EthTransaction transaction = await contract.CallFunction(functionName, functionArgs).Create(client, callingInfo);
            string result = await wallet.SendTransaction(client, transaction);
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
            this CallContractFunction transactionCreator,
            IWallet wallet,
            IEthClient client,
            BigInteger? value = null)
        {
            EthTransaction transaction = await transactionCreator.Create(client, new ContractCall(wallet.GetAddress(), value));
            string result = await wallet.SendTransaction(client, transaction);
            return result;
        }

        public static async Task<TransactionReceipt> SendTransactionMethodAndWaitForReceipt(
            this CallContractFunction transactionCreator,
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
