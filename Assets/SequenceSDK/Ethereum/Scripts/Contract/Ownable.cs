using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Extensions;
using Sequence.Provider;
using UnityEngine;
using static Sequence.Contracts.Contract;

namespace Sequence.Contracts
{
    public class Ownable
    {
        Contract contract;

        public Ownable(Contract contract)
        {
            this.contract = contract;
        }

        public Ownable(string contractAddress)
        {
            this.contract = new Contract(contractAddress);
        }

        public async Task<string> Owner(IEthClient client)
        {
            string result = await contract.SendQuery(client, "owner()");
            return result.Replace("0x", "").TrimStart('0').EnsureHexPrefix();
        }

        public CallContractFunctionTransactionCreator RenounceOwnership()
        {
            return contract.CallFunction("renounceOwnership()");
        }

        public CallContractFunctionTransactionCreator TransferOwnership(string toAddress)
        {
            return contract.CallFunction("transferOwnership(address)", toAddress);
        }
    }
}
