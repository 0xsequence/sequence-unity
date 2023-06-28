using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Provider;
using UnityEngine;

namespace Sequence.Contracts
{
    public class ERC20 : MonoBehaviour
    {
        Contract contract;

        public ERC20(Contract contract)
        {
            this.contract = contract;
        }

        public ERC20(string contractAddress)
        {
            this.contract = new Contract(contractAddress);
        }

        public Task<string> Name(IEthClient client) {
            return contract.QueryContract(client, new string[] { "name()"});
        }

        public Task<string> Symbol(IEthClient client)
        {
            return contract.QueryContract(client, new string[] { "symbol()" });
        }

        public Task<string> TotalSupply(IEthClient client)
        {
            return contract.QueryContract(client, new string[] { "totalSupply()" });
        }

        public Task<string> BalanceOf(IEthClient client, string address)
        {
            return contract.QueryContract(client, new string[] { "balanceOf()", address });
        }

        public Task<string> Allowance(IEthClient client, string ownerAddress, string spenderAddress)
        {
            return contract.QueryContract(client, new string[] { $"allowance({ownerAddress}, {spenderAddress})" });
        }
    }
}
