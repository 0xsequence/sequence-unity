using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    public class IntentResponseAccountList
    {
        public Account[] accounts;
        public string currentAccountId;
        
        [UnityEngine.Scripting.Preserve]
        public IntentResponseAccountList(Account[] accounts, string currentAccountId)
        {
            this.accounts = accounts;
            this.currentAccountId = currentAccountId;
        }
    }
}