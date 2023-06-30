using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Extensions;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;

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
            BigInteger nonce = await wallet.GetNonce(client);
            TransactionCall call = new TransactionCall
            {
                from = wallet.GetAddress(),
                value = 0,
                data = bytecode,
            };
            if (gasPrice == null)
            {
                gasPrice = await client.SuggestGasPrice();
            }
            if (gasLimit == null)
            {
                gasLimit = await client.EstimateGas(call);
            }
            EthTransaction deployTransaction = new EthTransaction(nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, StringExtensions.ZeroAddress, 0, bytecode);
            string signedTransaction = deployTransaction.SignAndEncodeTransaction(wallet);
            TransactionReceipt receipt = await wallet.SendRawTransactionAndWaitForReceipt(client, signedTransaction);
            return receipt;
        }
    }
}
