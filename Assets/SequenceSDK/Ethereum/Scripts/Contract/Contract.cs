using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Numerics;

namespace Sequence.Contracts
{
    public class Contract
    {
        string address;


        public Contract(string abi, string contractAddress)
        {

        }

        public async Task<string> Deploy(string bytecode, params object[] constructorArgs)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionReceipt> CallFunction(string functionName, params object[] functionArgs)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetEventLog<T>(string eventName, BigInteger blockNumber)
        {
            throw new NotImplementedException();
        }        
    }
}


