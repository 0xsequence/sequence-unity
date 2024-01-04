using Newtonsoft.Json;

namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class SendERC20 : SequenceSDK.WaaS.Transaction
    {
        public const string TypeIdentifier = "erc20send";
        public string to { get; private set; }
        public string token { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
        public string value { get; private set; }
        
        public SendERC20(string tokenAddress, string to, string value)
        {
            this.token = tokenAddress;
            this.to = to;
            this.value = value;
        }

        
        [JsonConstructor]
        public SendERC20(string to, string token, string type, string value)
        {
            this.to = to;
            this.token = token;
            this.type = type;
            this.value = value;
        }
    }
}