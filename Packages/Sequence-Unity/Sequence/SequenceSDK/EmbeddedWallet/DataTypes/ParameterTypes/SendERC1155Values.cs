using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class SendERC1155Values
    {
        public string amount;
        public string id;

        [UnityEngine.Scripting.Preserve]
        public SendERC1155Values(string id, string amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }
}