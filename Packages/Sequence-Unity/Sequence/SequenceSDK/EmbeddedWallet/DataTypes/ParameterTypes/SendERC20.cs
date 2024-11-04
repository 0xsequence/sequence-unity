using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class SendERC20 : Transaction
    {
        public const string TypeIdentifier = "erc20send";
        public string to;
        public string tokenAddress;
        public string type = TypeIdentifier;
        public string value;
        
        public SendERC20(string tokenAddress, string to, string value)
        {
            this.tokenAddress = tokenAddress;
            this.to = to;
            this.value = value;
        }

        [Preserve]
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