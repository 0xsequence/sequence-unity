using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;

namespace Sequence.Transactions {
    public class GasLimitEstimator {

        IEthClient client;
        string wallet;

        public delegate Task<EthTransaction> TransactionCreator();

        public GasLimitEstimator(IEthClient client, string wallet) {
            this.client = client;
            this.wallet = wallet;
        }

        public TransactionCreator BuildTransactionCreator(string to, string data = null, BigInteger? value = null, BigInteger? gasPrice = null) {
            return async () => {
                if (value == null)
                {
                    value = BigInteger.Zero;
                }

                TransactionCall call = new TransactionCall
                {
                    from = wallet,
                    to = to,
                    value = (BigInteger)value,
                    data = data,
                };
                if (gasPrice == null || gasPrice == BigInteger.Zero)
                {
                    gasPrice = await client.SuggestGasPrice();
                    call.gasPrice = (BigInteger)gasPrice;
                }
                BigInteger gasLimit = await client.EstimateGas(call);
                
                BigInteger nonce = await client.NonceAt(wallet);
                EthTransaction transaction = new EthTransaction(nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, to, (BigInteger)value, data);
                return transaction;
            };
        }
    }
}