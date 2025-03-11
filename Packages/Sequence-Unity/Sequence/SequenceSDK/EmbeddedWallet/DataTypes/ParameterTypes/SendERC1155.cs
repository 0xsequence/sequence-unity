using System;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class SendERC1155 : Transaction
    {
        public const string TypeIdentifier = "erc1155send";
        public string data;
        public string to;
        public string tokenAddress;
        public string type = TypeIdentifier;
        public SendERC1155Values[] vals;

        public SendERC1155(string tokenAddress, string to, SendERC1155Values[] sendErc1155Values, string data = null)
        {
            this.tokenAddress = tokenAddress;
            this.to = to;
            this.vals = sendErc1155Values;
            this.data = data;
        }

        [Preserve]
        [JsonConstructor]
        public SendERC1155(string data, string to, string tokenAddress, string type, SendERC1155Values[] vals)
        {
            this.data = data;
            this.to = to;
            this.tokenAddress = tokenAddress;
            this.type = type;
            this.vals = vals;
        }
    }
}