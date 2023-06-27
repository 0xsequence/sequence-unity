using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.Contracts
{
    public static class ContractTransactionSender
    {
        public static async Task<string> SendTransactionMethod(
            this Contract contract,
            EthWallet wallet,
            IEthClient client,
            string toAddress,
            BigInteger value,
            string functionName,
            params object[] functionArgs)
        {
            ContractCall callingInfo = new ContractCall(wallet.GetAddress(), toAddress, wallet.GetNonce(), value);
            EthTransaction transaction = await contract.CallFunction(functionName, functionArgs)(client, callingInfo);
            string signedTransaction = transaction.SignAndEncodeTransaction(wallet);
            string result = await wallet.SendRawTransaction(client, signedTransaction);
            return result;
        }

        public static async Task<TransactionReceipt> SendTransactionMethodAndWaitForReceipt(
            this Contract contract,
            EthWallet wallet,
            IEthClient client,
            string toAddress,
            BigInteger value,
            string functionName,
            params object[] functionArgs)
        {
            string transactionHash = await contract.SendTransactionMethod(wallet, client, toAddress, value, functionName, functionArgs);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
            return receipt;
        }
    }
}
