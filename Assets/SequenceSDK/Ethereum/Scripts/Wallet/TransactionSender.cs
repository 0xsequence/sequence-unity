using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;

namespace Sequence.Wallet {
    public static class TransactionSender {

        public static async Task<string> SendTransaction(this EthWallet fromWallet, IEthClient client, string to, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, string data = null) {
            if (value == null)
            {
                value = BigInteger.Zero;
            }
            if (gasPrice == null)
            {
                gasPrice = BigInteger.Zero;
            }
            if (gasLimit == null)
            {
                gasLimit = BigInteger.Zero;
            }

            TransactionCall call = new TransactionCall
            {
                from = fromWallet.GetAddress(),
                to = to,
                value = (BigInteger)value,
                data = data,
            };
            if (gasPrice == 0)
            {
                gasPrice = await client.SuggestGasPrice();
                call.gasPrice = (BigInteger)gasPrice;
            }
            if (gasLimit == 0)
            {
                gasLimit = await client.EstimateGas(call);
            }
            
            BigInteger nonce = await fromWallet.GetNonce(client);
            EthTransaction transaction = new EthTransaction(nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, to, (BigInteger)value, data);
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