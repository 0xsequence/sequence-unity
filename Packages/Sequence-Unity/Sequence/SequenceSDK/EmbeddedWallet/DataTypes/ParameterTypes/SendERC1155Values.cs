using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class SendERC1155Values
    {
        public string amount;
        public string id;

        public SendERC1155Values(string id, string amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }
}