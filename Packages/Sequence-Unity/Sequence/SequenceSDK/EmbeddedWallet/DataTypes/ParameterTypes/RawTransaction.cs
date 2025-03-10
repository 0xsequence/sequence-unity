using System.Numerics;
using Newtonsoft.Json;
using Sequence.Contracts;
using Sequence.Transactions;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class RawTransaction : Transaction
    {
        public const string TypeIdentifier = "transaction";
        public string data;
        public string to;
        public string type = TypeIdentifier;
        public string value;

        public RawTransaction(string to, string value = null, string calldata = null)
        {
            if (to == StringExtensions.ZeroAddress)
            {
                to = ZeroAddress;
            }
            this.to = to;
            this.value = value;
            this.data = calldata;
        }

        public RawTransaction(EthTransaction transaction)
        {
            string to = transaction.To;
            if (to == StringExtensions.ZeroAddress)
            {
                to = ZeroAddress;
            }
            this.to = to;
            this.value = transaction.Value.ToString();
            this.data = transaction.Data;
        }
        
        public RawTransaction(Contract contract, string functionName, params object[] functionArgs)
        {
            string to = contract.GetAddress();
            if (to == StringExtensions.ZeroAddress)
            {
                to = ZeroAddress;
            }
            this.to = to;
            this.value = BigInteger.Zero.ToString();
            this.data = contract.AssembleCallData(functionName, functionArgs);
        }

        public RawTransaction(CallContractFunction callContractFunction, string value = "0")
        {
            string to = callContractFunction.Address;
            if (to == StringExtensions.ZeroAddress)
            {
                to = ZeroAddress;
            }

            this.to = to;
            this.value = value;
            this.data = callContractFunction.CallData;
        }
        
        [Preserve]
        [JsonConstructor]
        public RawTransaction(string data, string to, string type, string value)
        {
            this.data = data;
            this.to = to;
            this.type = type;
            this.value = value;
        }
    }
}
