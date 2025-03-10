using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    public class IntentResponseAccountFederated
    {
        public Account account;

        [UnityEngine.Scripting.Preserve]
        public IntentResponseAccountFederated(Account account)
        {
            this.account = account;
        }
    }
}