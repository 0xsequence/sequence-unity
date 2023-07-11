using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;
using static Sequence.Contracts.Contract;

namespace Sequence.Contracts
{
    public static class ContractQuerySender
    {
        public static async Task<string> SendQuery(
            this Contract contract,
            IEthClient client,
            string functionSignature,
            params object[] functionArgs)
        {
            string result = await contract.QueryContract(functionSignature, functionArgs)(client);
            return result;
        }

        public static async Task<string> SendQuery(
            this QueryContractMessageSender querySender,
            IEthClient client)
        {
            string result = await querySender(client);
            return result;
        }
    }
}
