namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class IntentResponseAccountFederated
    {
        public Account account;

        [Preserve]
        public IntentResponseAccountFederated(Account account)
        {
            this.account = account;
        }
    }
}