using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class SendERC1155Values
    {
        public string id { get; private set; }
        public string amount { get; private set; }

        public SendERC1155Values(string id, string amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }
}