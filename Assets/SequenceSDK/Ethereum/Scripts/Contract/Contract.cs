using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Provider;
using Sequence.Extensions;
using Sequence.Transactions;
using System.Text;

namespace Sequence.Contracts
{
    public class Contract
    {
        string address;
        public delegate Task<EthTransaction> CallContractFunctionTransactionCreator(IEthClient client, ContractCall contractCallInfo);
        public delegate Task<string> QueryContractMessageSender(IEthClient client);

        public Contract(string contractAddress)
        {
            address = contractAddress;
        }

        public async Task<string> Deploy(string bytecode, params object[] constructorArgs)
        {
            throw new NotImplementedException();
        }

        public CallContractFunctionTransactionCreator CallFunction(string functionSignature, params object[] functionArgs)
        {
            string callData = ABI.ABI.Pack(functionSignature, functionArgs);
            return async (IEthClient client, ContractCall contractCallInfo) =>
            {
                TransactionCall call = new TransactionCall
                {
                    from = contractCallInfo.fromAddress,
                    to = this.address,
                    value = contractCallInfo.value,
                    data = callData,
                };
                if (contractCallInfo.gasPrice == 0)
                {
                    contractCallInfo.gasPrice = await client.SuggestGasPrice();
                    call.gasPrice = contractCallInfo.gasPrice;
                }
                if (contractCallInfo.gasLimit == 0)
                {
                    contractCallInfo.gasLimit = await client.EstimateGas(call);
                }

                BigInteger nonce = await client.NonceAt(contractCallInfo.fromAddress);

                EthTransaction transaction = new EthTransaction(
                    nonce,
                    call.gasPrice,
                    contractCallInfo.gasLimit,
                    call.to,
                    call.value,
                    callData);

                return transaction;
            };
        }

        public QueryContractMessageSender QueryContract(string functionSignature, params object[] args)
        {
            string data = ABI.ABI.Pack(functionSignature, args);
            string to = address;
            object[] toSendParams = new object[] {
                new
                {
                    to,
                    data
                }
            };
            return async (IEthClient client) =>
            {
                return await client.CallContract(toSendParams);
            };
        }

        public async Task<T> GetEventLog<T>(string eventName, BigInteger blockNumber)
        {
            throw new NotImplementedException();
        }        
    }

    public class ContractCall
    {
        public string fromAddress;
        public BigInteger value;
        public BigInteger gasPrice;
        public BigInteger gasLimit;

        public ContractCall(string fromAddress, BigInteger? value = null, BigInteger? gasPrice = null, BigInteger? gasLimit = null)
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

            if (!fromAddress.IsAddress())
            {
                throw new ArgumentOutOfRangeException(nameof(fromAddress));
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

            this.fromAddress = fromAddress;
            this.value = (BigInteger)value;
            this.gasPrice = (BigInteger)gasPrice;
            this.gasLimit = (BigInteger)gasLimit;
        }
    }
}


