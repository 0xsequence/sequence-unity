using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.Utils;

namespace Sequence.Transactions {
    public class GasLimitEstimator {

        IEthClient client;
        Address wallet;

        public delegate Task<EthTransaction> TransactionCreator();

        public GasLimitEstimator(IEthClient client, Address wallet) {
            this.client = client;
            this.wallet = wallet;
        }

        /// <summary>
        /// Build a TransactionCreator delegate that, when called, will return an EthTransaction with estimated gasPrice and gasLimit and the wallet's current nonce
        /// </summary>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="gasPrice"></param>
        /// <param name="gasLimit"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public TransactionCreator BuildTransactionCreator(string to, string data = null, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, BigInteger? nonce = null) {
            return async () => {
                if (value == null)
                {
                    value = BigInteger.Zero;
                }

                TransactionCall call = new TransactionCall
                {
                    from = wallet,
                    value = (BigInteger)value,
                    data = data,
                };
                if (to != StringExtensions.ZeroAddress)
                {
                    call.to = to;
                }

                if (gasPrice == null || gasPrice == BigInteger.Zero)
                {
                    gasPrice = await client.SuggestGasPrice();
                    call.gasPrice = (BigInteger)gasPrice;
                }
                if (gasLimit == null || gasLimit == BigInteger.Zero)
                {
                    gasLimit = await client.EstimateGas(call);
                }
                if (nonce == null)
                {
                    nonce = await client.NonceAt(wallet);
                }
                
                string chainId = await client.ChainID();
                EthTransaction transaction = new EthTransaction((BigInteger)nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, to, (BigInteger)value, data, chainId);
                return transaction;
            };
        }

        /// <summary>
        /// Await to receive an EthTransaction with estimated gasPrice and gasLimit and the wallet's current nonce
        /// </summary>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="gasPrice"></param>
        /// <param name="gasLimit"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public async Task<EthTransaction> BuildTransaction(string to, string data = null, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, BigInteger? nonce = null)
        {
            return await BuildTransactionCreator(to, data, value, gasPrice, gasLimit, nonce)();
        }
    }
}