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

namespace Sequence.Signer
{
    public class TransferEth
    {
        private EthWallet fromWallet;
        private string to;
        private BigInteger value;
        private BigInteger gasPrice;
        private BigInteger gasLimit;
        private BigInteger nonce;

        public TransferEth(
            EthWallet fromWallet,
            string toAddress,
            BigInteger value,
            BigInteger gasPrice,
            BigInteger gasLimit,
            BigInteger nonce)
        {
            if (fromWallet == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fromWallet));
            }
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            EthTransaction.ValidateParams(toAddress, value, gasPrice, gasLimit, nonce);
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
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<string> Send(SequenceEthClient client)
        {
            string encoded_signing = EthTransaction.RLPEncode(fromWallet.GetNonce(), gasPrice, gasLimit, to, value, null);
            string signingHash = SequenceCoder.KeccakHash(encoded_signing).EnsureHexPrefix();
            (string v, string r, string s) = fromWallet.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash));
            string tx = EthTransaction.RLPEncode(fromWallet.GetNonce(), gasPrice, gasLimit, to, value, null, v, r, s);
            string result = await fromWallet.SendRawTransaction(client, tx);
            return result;
        }

        /// <summary>
        /// Signs and sends the Eth transfer transaction then waits for and returns a transaction receipt from the client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<TransactionReceipt> SendAndWaitForReceipt(SequenceEthClient client)
        {
            string transactionHash = await Send(client);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(transactionHash);
            return receipt;
        }
    }
}