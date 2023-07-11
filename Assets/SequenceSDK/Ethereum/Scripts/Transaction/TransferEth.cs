using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.Transactions
{
    public class TransferEth
    {
        private IEthClient client;
        private EthWallet fromWallet;
        private string to;
        private BigInteger value;
        private BigInteger gasPrice;
        private BigInteger gasLimit;
        private BigInteger nonce;

        public TransferEth(
            IEthClient client,
            EthWallet fromWallet,
            string toAddress,
            BigInteger value,
            BigInteger gasPrice,
            BigInteger gasLimit,
            BigInteger nonce)
        {
            if (client == null)
            {
                throw new ArgumentOutOfRangeException(nameof(client));
            }
            if (fromWallet == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fromWallet));
            }
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            EthTransaction.ValidateParams(toAddress, value, gasPrice, gasLimit, nonce);
            this.client = client;
            this.fromWallet = fromWallet;
            this.to = toAddress;
            this.value = value;
            this.gasPrice = gasPrice;
            this.gasLimit = gasLimit;
            this.nonce = nonce;
        }

        /// <summary>
        /// Signs and sends the Eth transfer transaction
        /// </summary>
        /// <returns></returns>
        public async Task<string> Send()
        {
            string result = await fromWallet.SendTransaction(client, to, value, gasPrice, gasLimit);
            return result;
        }

        /// <summary>
        /// Signs and sends the Eth transfer transaction then waits for and returns a transaction receipt from the client
        /// </summary>
        /// <returns></returns>
        public async Task<TransactionReceipt> SendAndWaitForReceipt()
        {
            string transactionHash = await Send();
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
            return receipt;
        }
    }
}