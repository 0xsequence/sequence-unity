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
        /// <summary>
        /// Send a query, specified by the functionSignature and functionArgs, on a smart contract
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="client"></param>
        /// <param name="functionSignature"></param>
        /// <param name="functionArgs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> SendQuery<T>(
            this Contract contract,
            IEthClient client,
            string functionSignature,
            params object[] functionArgs)
        {
            T result = await contract.QueryContract<T>(functionSignature, functionArgs)(client);
            return result;
        }

        /// <summary>
        /// Send a query on a smart contract
        /// </summary>
        /// <param name="querySender"></param>
        /// <param name="client"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> SendQuery<T>(
            this QueryContractMessageSender<T> querySender,
            IEthClient client)
        {
            T result = await querySender(client);
            return result;
        }
    }
}
