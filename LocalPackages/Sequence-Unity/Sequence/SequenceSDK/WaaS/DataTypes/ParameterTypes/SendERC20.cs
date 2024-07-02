using Newtonsoft.Json;

namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class SendERC20 : Sequence.WaaS.Transaction
    {
        public const string TypeIdentifier = "erc20send";
        public string to { get; private set; }
        public string tokenAddress { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
        public string value { get; private set; }
        
        public SendERC20(string tokenAddress, string to, string value)
        {
            this.tokenAddress = tokenAddress;
            this.to = to;
            this.value = value;
        }

        
        [JsonConstructor]
        public SendERC20(string to, string tokenAddress, string type, string value)
        {
            this.to = to;
            this.tokenAddress = tokenAddress;
            this.type = type;
            this.value = value;
        }
    }
}