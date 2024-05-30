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
        /// <summary>
        /// Send a transaction calling a method on a smart contract identified by the functionName and functionArgs
        /// Using the wallet's current nonce and estimating the gasPrice and gasLimit
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="wallet"></param>
        /// <param name="client"></param>
        /// <param name="value"></param>
        /// <param name="functionName"></param>
        /// <param name="functionArgs"></param>
        /// <returns>Transaction hash string</returns>
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

        /// <summary>
        /// Send a transaction calling a method on a smart contract identified by the functionName and functionArgs
        /// Using the wallet's current nonce and estimating the gasPrice and gasLimit
        /// Then continually poll the IEthClient for a TransactionReceipt
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="wallet"></param>
        /// <param name="client"></param>
        /// <param name="value"></param>
        /// <param name="functionName"></param>
        /// <param name="functionArgs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Send a transaction calling a method on a smart contract identified by the functionName and functionArgs
        /// Using the wallet's current nonce and estimating the gasPrice and gasLimit
        /// </summary>
        /// <param name="transactionCreator"></param>
        /// <param name="wallet"></param>
        /// <param name="client"></param>
        /// <param name="value"></param>
        /// <returns>Transaction hash string</returns>
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

        /// <summary>
        /// Send a transaction calling a method on a smart contract identified by the functionName and functionArgs
        /// Using the wallet's current nonce and estimating the gasPrice and gasLimit
        /// Then continually poll the IEthClient for a TransactionReceipt
        /// </summary>
        /// <param name="transactionCreator"></param>
        /// <param name="wallet"></param>
        /// <param name="client"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
