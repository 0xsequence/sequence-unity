using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class SendERC1155Values
    {
        public string amount;
        public string id;

        [Preserve]
        public SendERC1155Values(string id, string amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }
}