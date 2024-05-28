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

        public async Task<EthTransaction> BuildTransaction(string to, string data = null, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null, BigInteger? nonce = null)
        {
            return await BuildTransactionCreator(to, data, value, gasPrice, gasLimit, nonce)();
        }
    }
}