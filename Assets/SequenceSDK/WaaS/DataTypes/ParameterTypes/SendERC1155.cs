using System;
using Newtonsoft.Json;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [Serializable]
    public class SendERC1155 : SequenceSDK.WaaS.Transaction
    {
        public const string TypeIdentifier = "erc1155send";
        public string data { get; private set; }
        public string to { get; private set; }
        public string token { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
        public SendERC1155Values[] vals { get; private set; }

        public SendERC1155(string tokenAddress, string to, SendERC1155Values[] sendErc1155Values, string data)
        {
            this.token = tokenAddress;
            this.to = to;
            this.vals = sendErc1155Values;
            this.data = data;
        }

        [JsonConstructor]
        public SendERC1155(string data, string to, string token, string type, SendERC1155Values[] vals)
        {
            this.data = data;
            this.to = to;
            this.token = token;
            this.type = type;
            this.vals = vals;
        }
    }
}