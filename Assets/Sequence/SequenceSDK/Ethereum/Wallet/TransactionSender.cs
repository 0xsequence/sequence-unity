using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Transactions;

namespace Sequence.Wallet {
    public static class TransactionSender {

        /// <summary>
        /// Send a transaction, estimating the gasPrice and gasLimit if none is provided, using the fromWallet's current nonce
        /// </summary>
        /// <param name="fromWallet"></param>
        /// <param name="client"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="gasPrice"></param>
        /// <param name="gasLimit"></param>
        /// <param name="data"></param>
        /// <returns>Transaction hash string</returns>
        public static async Task<string> SendTransaction(this IWallet fromWallet, IEthClient client, string to, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, string data = null) {
            EthTransaction transaction;
            string chainId = await client.ChainID();
            if (gasLimit == null) {
                GasLimitEstimator estimator = new GasLimitEstimator(client, fromWallet.GetAddress());
                transaction = await estimator.BuildTransactionCreator(to, data, value, gasPrice)();
            }else
            {
                BigInteger nonce = await client.NonceAt(fromWallet.GetAddress());
                transaction = new EthTransaction(nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, to, (BigInteger)value, data, chainId);
            }
            string result = await fromWallet.SendTransaction(client, transaction);
            return result;
        }

        /// <summary>
        /// Send a transaction, estimating the gasPrice and gasLimit if none is provided, using the fromWallet's current nonce
        /// Then, continuously poll the IEthClient for a TransactionReceipt
        /// </summary>
        /// <param name="fromWallet"></param>
        /// <param name="client"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="gasPrice"></param>
        /// <param name="gasLimit"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<TransactionReceipt> SendTransactionAndWaitForReceipt(this IWallet fromWallet, IEthClient client, string to, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, string data = null) {
            string tx = await fromWallet.SendTransaction(client, to, value, gasPrice, gasLimit, data);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(tx);
            return receipt;
        }

    }
}