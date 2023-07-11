using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Extensions;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;
using Sequence.Transactions;

namespace Sequence.Contracts
{
    public static class ContractDeployer
    {
        public static async Task<TransactionReceipt> Deploy(
            IEthClient client,
            EthWallet wallet,
            string bytecode,
            BigInteger? gasPrice = null,
            BigInteger? gasLimit = null)
        {
            EthTransaction deployTransaction = await new GasLimitEstimator(client, wallet.GetAddress()).BuildTransaction(StringExtensions.ZeroAddress, bytecode, 0, gasPrice, gasLimit);
            string signedTransaction = deployTransaction.SignAndEncodeTransaction(wallet);
            TransactionReceipt receipt = await wallet.SendRawTransactionAndWaitForReceipt(client, signedTransaction);
            return receipt;
        }
    }
}
