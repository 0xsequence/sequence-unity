using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Provider;
using Sequence.Transactions;
using System.Text;
using UnityEngine;

namespace Sequence.Contracts
{
    public class Contract
    {
        Address address;
        public delegate Task<EthTransaction> CallContractFunctionTransactionCreator(IEthClient client, ContractCall contractCallInfo);
        public delegate Task<T> QueryContractMessageSender<T>(IEthClient client);

        private string abi;
        private FunctionAbi functionAbi;

        /// <summary>
        /// Will throw if given an invalid abi or contractAddress
        /// Note that providing a null abi is supported. However this is not recommended.
        /// Using a null abi will require you to provide the full function signature when transacting/querying the contract.
        /// Using a null abi will cause all query responses to return as a string.
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="abi"></param>
        public Contract(string contractAddress, string abi = null)
        {
            address = new Address(contractAddress);
            this.abi = abi;
            if (abi == null)
            {
                Debug.LogWarning("Creating a contract with a null ABI, while supported, is not recommended. Note: Using a null abi will require you to provide the full function signature when transacting/querying the contract. Using a null abi will cause all query responses to return as a string.");
                this.functionAbi = null;
            }
            else
            {
                this.functionAbi = ABI.ABI.DecodeAbi(abi);
            }
        }

        public async Task<string> Deploy(string bytecode, params object[] constructorArgs)
        {
            throw new NotImplementedException();
        }

        public CallContractFunctionTransactionCreator CallFunction(string functionName, params object[] functionArgs)
        {
            string callData = GetData(functionName, functionArgs);
            return async (IEthClient client, ContractCall contractCallInfo) =>
            {
                GasLimitEstimator estimator = new GasLimitEstimator(client, contractCallInfo.from);
                var transactionCreator = estimator.BuildTransactionCreator(this.address, callData, contractCallInfo.value, contractCallInfo.gasPrice);

                EthTransaction transaction = await transactionCreator();
                return transaction;
            };
        }

        public QueryContractMessageSender<T> QueryContract<T>(string functionName, params object[] args)
        {
            if (this.functionAbi == null)
            {
                // Return string, throw exception is anything else is provided as T
                if (typeof(T) != typeof(string))
                {
                    throw new ArgumentException(
                        "Unsupported method call. If contract ABI is null, can only return string as a query response.");
                }
                return (IEthClient client) => QueryContract(functionName, args)(client).ContinueWith(t => (T)(object)t.Result.ToString());
            }
            
            object[] toSendParams = CreateParams(functionName, args);
            return async (IEthClient client) =>
            {
                // Instead, get the string response, then use ABI (define a method in FunctionABI) to decode the hex string into the correct datatype
                string result = await client.CallContract(toSendParams);
                return this.functionAbi.DecodeReturnValue<T>(result, functionName, args);
            };
        }
        
        private QueryContractMessageSender<string> QueryContract(string functionName, params object[] args)
        {
            object[] toSendParams = CreateParams(functionName, args);
            return async (IEthClient client) =>
            {
                return await client.CallContract(toSendParams);
            };
        }

        private object[] CreateParams(string functionName, params object[] args)
        {
            string data = GetData(functionName, args);
            string to = address;
            object[] toSendParams = new object[] {
                new
                {
                    to,
                    data
                }
            };
            return toSendParams;
        }
        private string GetData(string functionName, params object[] args)
        {
            string data;
            if (this.functionAbi != null)
            {
                int abiIndex = this.functionAbi.GetFunctionAbiIndex(functionName, args);
                data = ABI.ABI.Pack(this.functionAbi.GetFunctionSignature(functionName, abiIndex), args);
            }
            else
            {
                data = ABI.ABI.Pack(functionName, args);
            }
            return data;
        }

        public async Task<T> GetEventLog<T>(string eventName, BigInteger blockNumber)
        {
            throw new NotImplementedException();
        }        
    }

    public class ContractCall
    {
        public Address from;
        public BigInteger value;
        public BigInteger gasPrice;
        public BigInteger gasLimit;

        public ContractCall(Address from, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null)
        {
            if (value == null)
            {
                value = BigInteger.Zero;
            }
            if (gasPrice == null)
            {
                gasPrice = BigInteger.Zero;
            }
            if (gasLimit == null)
            {
                gasLimit = BigInteger.Zero;
            }
            
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (gasPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasPrice));
            }
            if (gasLimit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasLimit));
            }

            this.from = from;
            this.value = (BigInteger)value;
            this.gasPrice = (BigInteger)gasPrice;
            this.gasLimit = (BigInteger)gasLimit;
        }
    }
}


