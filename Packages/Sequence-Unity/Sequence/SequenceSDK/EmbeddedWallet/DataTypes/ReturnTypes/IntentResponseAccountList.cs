namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class IntentResponseAccountList
    {
        public Account[] accounts;
        public string currentAccountId;
        
        [Preserve]
        public IntentResponseAccountList(Account[] accounts, string currentAccountId)
        {
            this.accounts = accounts;
            this.currentAccountId = currentAccountId;
        }
    }
}