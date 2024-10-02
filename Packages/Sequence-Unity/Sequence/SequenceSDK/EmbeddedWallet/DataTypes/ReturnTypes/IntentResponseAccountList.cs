namespace Sequence.EmbeddedWallet
{
    public class IntentResponseAccountList
    {
        public Account[] accounts;
        public string currentAccountId;
        
        public IntentResponseAccountList(Account[] accounts, string currentAccountId)
        {
            this.accounts = accounts;
            this.currentAccountId = currentAccountId;
        }
    }
}