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
        public static async Task<T> SendQuery<T>(
            this Contract contract,
            IEthClient client,
            string functionSignature,
            params object[] functionArgs)
        {
            T result = await contract.QueryContract<T>(functionSignature, functionArgs)(client);
            return result;
        }

        public static async Task<T> SendQuery<T>(
            this QueryContractMessageSender<T> querySender,
            IEthClient client)
        {
            T result = await querySender(client);
            return result;
        }
    }
}
