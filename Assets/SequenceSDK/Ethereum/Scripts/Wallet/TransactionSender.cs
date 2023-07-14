using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Transactions;

namespace Sequence.Wallet {
    public static class TransactionSender {

        public static async Task<string> SendTransaction(this EthWallet fromWallet, IEthClient client, string to, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, string data = null) {
            EthTransaction transaction;
            string chainId = await client.ChainID();
            if (gasLimit == null) {
                GasLimitEstimator estimator = new GasLimitEstimator(client, fromWallet.GetAddress());
                transaction = await estimator.BuildTransactionCreator(to, data, value, gasPrice)();
            }else {
                BigInteger nonce = await fromWallet.GetNonce(client);
                transaction = new EthTransaction(nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, to, (BigInteger)value, data, chainId);
            }
            string tx = transaction.SignAndEncodeTransaction(fromWallet);
            string result = await fromWallet.SendRawTransaction(client, tx);
            return result;
        }

        public static async Task<TransactionReceipt> SendTransactionAndWaitForReceipt(this EthWallet fromWallet, IEthClient client, string to, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, string data = null) {
            string tx = await fromWallet.SendTransaction(client, to, value, gasPrice, gasLimit, data);
            TransactionReceipt receipt = await client.WaitForTransactionReceipt(tx);
            return receipt;
        }

    }
}