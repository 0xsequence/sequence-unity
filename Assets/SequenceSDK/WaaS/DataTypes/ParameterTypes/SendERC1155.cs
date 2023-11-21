using System;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [Serializable]
    public class SendERC1155 : SequenceSDK.WaaS.Transaction
    {
        public string data { get; private set; }
        public string to { get; private set; }
        public string token { get; private set; }
        public string type { get; private set; } = "erc1155send";
        public SendERC1155Values[] vals { get; private set; }

        public SendERC1155(string tokenAddress, string to, SendERC1155Values[] sendErc1155Values, string data)
        {
            this.token = tokenAddress;
            this.to = to;
            this.vals = sendErc1155Values;
            this.data = data;
        }
    }
}