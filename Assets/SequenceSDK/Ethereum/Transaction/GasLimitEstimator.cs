using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.Extensions;

namespace Sequence.Transactions {
    public class GasLimitEstimator {

        IEthClient client;
        string wallet;

        public delegate Task<EthTransaction> TransactionCreator();

        public GasLimitEstimator(IEthClient client, string wallet) {
            this.client = client;
            this.wallet = wallet;
        }

        public TransactionCreator BuildTransactionCreator(string to, string data = null, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null) {
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
                
                BigInteger nonce = await client.NonceAt(wallet);
                string chainId = await client.ChainID();
                EthTransaction transaction = new EthTransaction(nonce, (BigInteger)gasPrice, (BigInteger)gasLimit, to, (BigInteger)value, data, chainId);
                return transaction;
            };
        }

        public async Task<EthTransaction> BuildTransaction(string to, string data = null, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null)
        {
            return await BuildTransactionCreator(to, data, value, gasPrice, gasLimit)();
        }
    }
}