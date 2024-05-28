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

        public async Task<EthTransaction> Create(IEthClient client, ContractCall contractCall)
        {
            GasLimitEstimator estimator = new GasLimitEstimator(client, contractCall.from);
            EthTransaction transaction = await estimator.BuildTransaction(Address, CallData, contractCall.value, contractCall.gasPrice);
            return transaction;
        }

        public async Task<string> Invoke(IEthClient client, IWallet wallet, BigInteger? value = null,
            BigInteger? gasPrice = null, BigInteger? gasLimit = null)
        {
            EthTransaction transaction =
                await Create(client, new ContractCall(wallet.GetAddress(), value, gasPrice, gasLimit));
            return await wallet.SendTransaction(client, transaction);
        }
    }
}