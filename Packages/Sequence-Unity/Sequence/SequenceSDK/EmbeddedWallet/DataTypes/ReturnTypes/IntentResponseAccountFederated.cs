using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class IntentResponseAccountFederated
    {
        public Account account;

        public IntentResponseAccountFederated(Account account)
        {
            this.account = account;
        }
    }
}