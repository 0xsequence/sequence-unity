using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Provider;
using Sequence.Extensions;
using Sequence.Wallet;
using System.Text;

namespace Sequence.Contracts
{
    public class Contract
    {
        string address;
        public delegate Task<EthTransaction> CallContractFunctionTransactionCreator(IEthClient client, ContractCall contractCallInfo);


        public Contract(string contractAddress)
        {
            address = contractAddress;
        }

        public async Task<string> Deploy(string bytecode, params object[] constructorArgs)
        {
            throw new NotImplementedException();
        }

        public CallContractFunctionTransactionCreator CallFunction(string functionName, params object[] functionArgs)
        {
            string callData = ABI.ABI.Pack($"function {functionName}({functionArgs})");
            return async (IEthClient client, ContractCall contractCallInfo) =>
            {
                TransactionCall call = new TransactionCall
                {
                    from = contractCallInfo.fromAddress,
                    to = contractCallInfo.toAddress,
                    value = contractCallInfo.value,
                };
                string blockNumber = await client.BlockNumber();
                BigInteger gasLimitEstimate = await client.EstimateGas(call, blockNumber);

                if (contractCallInfo.gasPrice == 0)
                {
                    contractCallInfo.gasPrice = await client.SuggestGasPrice();
                }
                call.gasPrice = contractCallInfo.gasPrice;

                EthTransaction transaction = new EthTransaction(
                    contractCallInfo.nonce,
                    call.gasPrice,
                    gasLimitEstimate,
                    call.to,
                    call.value,
                    callData);

                return transaction;
            };
        }

        public async Task<string> QueryContract(IEthClient client, string[] args)
        {
            //byte[] bytes = Encoding.UTF8.GetBytes(args);
            byte[] concatenatedBytes = new byte[0];
            int numberOfArgs = args.Length;
            for (int i = 0; i < numberOfArgs; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(args[i]);
                byte[] temp = new byte[concatenatedBytes.Length + bytes.Length];
                concatenatedBytes.CopyTo(temp, 0);
                bytes.CopyTo(temp, concatenatedBytes.Length);
                concatenatedBytes = temp;
            }
            byte[] hashedBytes = SequenceCoder.KeccakHash(concatenatedBytes);
            string data = SequenceCoder.ByteArrayToHexString(hashedBytes).EnsureHexPrefix();
            string to = address;
            object[] toSendParams = new object[] {
                new
                {
                    to,
                    data
                }
            };
            return await client.CallContract(this.address, toSendParams);
        }

        public async Task<T> GetEventLog<T>(string eventName, BigInteger blockNumber)
        {
            throw new NotImplementedException();
        }        
    }

    public class ContractCall
    {
        public string fromAddress;
        public string toAddress;
        public BigInteger nonce;
        public BigInteger value;
        public BigInteger gasPrice;

        public ContractCall(string fromAddress, string toAddress, BigInteger nonce, BigInteger? value = null, BigInteger? gasPrice = null)
        {
            if (value == null)
            {
                value = BigInteger.Zero;
            }
            if (gasPrice == null)
            {
                gasPrice = BigInteger.Zero;
            }

            if (!fromAddress.IsAddress())
            {
                throw new ArgumentOutOfRangeException(nameof(fromAddress));
            }
            if (!toAddress.IsAddress())
            {
                throw new ArgumentOutOfRangeException(nameof(toAddress));
            }
            if (nonce < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nonce));
            }
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (gasPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasPrice));
            }

            this.fromAddress = fromAddress;
            this.toAddress = toAddress;
            this.value = (BigInteger)value;
        }
    }
}


