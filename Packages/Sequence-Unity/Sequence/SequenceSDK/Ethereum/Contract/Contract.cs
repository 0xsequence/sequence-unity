using System.Threading.Tasks;
using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Provider;
using Sequence.Utils;

namespace Sequence.Contracts
{
    public class Contract
    {
        private struct QueryParameters
        {
            public Address to;
            public string data;
        }
        
        Address address;
        public delegate Task<T> QueryContractMessageSender<T>(IEthClient client);

        private string _abi;
        private FunctionAbi _functionAbi;

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
            this._abi = abi;
            if (abi == null)
            {
                SequenceLog.Warning("Creating a contract with a null ABI is not recommended. Note: Using a null abi will require you to provide the full function signature when transacting/querying the contract. Using a null abi will cause all query responses to return as a string.");
                this._functionAbi = null;
            }
            else
            {
                this._functionAbi = ABI.ABI.DecodeAbi(abi);
            }
        }

        public Address GetAddress()
        {
            return address;
        }
        
        public static implicit operator Address(Contract contract)
        {
            return contract.GetAddress();
        }

        public async Task<string> Deploy(string bytecode, params object[] constructorArgs)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Encode the call data for the given functionName and functionArgs
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="functionArgs"></param>
        /// <returns></returns>
        public string AssembleCallData(string functionName, params object[] functionArgs)
        {
            functionName = ValidateFunctionNameRegex(functionName);
            return GetData(functionName, functionArgs);
        }

        private string ValidateFunctionNameRegex(string functionName)
        {
            if (string.IsNullOrWhiteSpace(_abi))
            {
                if (!ABIRegex.MatchesFunctionABI(functionName))
                {
                    string message =
                        $"Given invalid {nameof(functionName)}, given: {functionName}; expected to regex match {ABIRegex.FunctionABIRegex} - for example: \"mint(uint256,uint256)\"";
                    SequenceLog.Warning(message + "\nAttempting to recover and parse anyways");
                    functionName = EventParser.ParseEventDef(functionName).ToString();
                    if (!ABIRegex.MatchesFunctionABI(functionName))
                    {
                        throw new ArgumentException(message);
                    }
                }
            }
            else
            {
                if (!ABIRegex.MatchesFunctionName(functionName))
                {
                    throw new ArgumentException($"Given invalid {nameof(functionName)}, given: {functionName}; expected to regex match {ABIRegex.FunctionNameRegex} - for example: \"mint\"");
                }
            }

            return functionName;
        }

        /// <summary>
        /// Create a CallContractFunction for the given functionName and functionArgs
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="functionArgs"></param>
        /// <returns></returns>
        public CallContractFunction CallFunction(string functionName, params object[] functionArgs)
        {
            functionName = ValidateFunctionNameRegex(functionName);
            string callData = GetData(functionName, functionArgs);
            return new CallContractFunction(callData, address);
        }

        /// <summary>
        /// Create a QueryContractMessageSender<T> delegate for the given query functionName and args
        /// The delegate, when called with an IEthClient, will perform the designated query
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public QueryContractMessageSender<T> QueryContract<T>(string functionName, params object[] args)
        {
            functionName = ValidateFunctionNameRegex(functionName);
            if (this._functionAbi == null)
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
                return this._functionAbi.DecodeReturnValue<T>(result, functionName, args);
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
            object[] toSendParams = new object[] {
                new QueryParameters
                {
                    to = address,
                    data = data
                }
            };
            return toSendParams;
        }
        private string GetData(string functionName, params object[] args)
        {
            string data;
            if (this._functionAbi != null)
            {
                int abiIndex = this._functionAbi.GetFunctionAbiIndex(functionName, args);
                data = ABI.ABI.Pack(this._functionAbi.GetFunctionSignature(functionName, abiIndex), args);
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


