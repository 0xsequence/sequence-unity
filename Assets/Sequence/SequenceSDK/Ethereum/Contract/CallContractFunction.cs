using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Wallet;

namespace Sequence.Contracts
{
    public class CallContractFunction
    {
        public string CallData;
        public Address Address;
        
        public CallContractFunction(string callData, Address address)
        {
            this.CallData = callData;
            this.Address = address;
        }

        /// <summary>
        /// Create an EthTransaction with estimated gasPrice and gasLimit using the calling wallet's current nonce for a given ContractCall
        /// </summary>
        /// <param name="client"></param>
        /// <param name="contractCall"></param>
        /// <returns></returns>
        public async Task<EthTransaction> Create(IEthClient client, ContractCall contractCall)
        {
            GasLimitEstimator estimator = new GasLimitEstimator(client, contractCall.from);
            EthTransaction transaction = await estimator.BuildTransaction(Address, CallData, contractCall.value, contractCall.gasPrice);
            return transaction;
        }

        /// <summary>
        /// Send a transaction calling the contract using the wallet's current nonce, estimating the gasPrice and gasLimit if not provided
        /// </summary>
        /// <param name="client"></param>
        /// <param name="wallet"></param>
        /// <param name="value"></param>
        /// <param name="gasPrice"></param>
        /// <param name="gasLimit"></param>
        /// <returns>Transaction hash string</returns>
        public async Task<string> Invoke(IEthClient client, IWallet wallet, BigInteger? value = null,
            BigInteger? gasPrice = null, BigInteger? gasLimit = null)
        {
            EthTransaction transaction =
                await Create(client, new ContractCall(wallet.GetAddress(), value, gasPrice, gasLimit));
            return await wallet.SendTransaction(client, transaction);
        }
    }
}