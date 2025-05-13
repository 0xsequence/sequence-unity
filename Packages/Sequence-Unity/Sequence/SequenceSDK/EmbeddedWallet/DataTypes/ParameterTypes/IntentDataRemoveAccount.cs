using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    internal class IntentDataRemoveAccount
    {
        public string accountId;
        public string wallet;

        [Preserve]
        public IntentDataRemoveAccount(string wallet, string accountId)
        {
            this.wallet = wallet;
            this.accountId = accountId;
        }
    }
}