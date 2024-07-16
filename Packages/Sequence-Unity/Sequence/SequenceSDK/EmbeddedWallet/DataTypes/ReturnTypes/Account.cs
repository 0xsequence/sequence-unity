namespace Sequence.EmbeddedWallet {
    public class Account
    {
        public string email;
        public string id;
        public IdentityType identityType;
        public string issuer;
        
        public Account(string id, string identityType, string issuer, string email)
        {
            this.id = id;
            this.identityType = identityType.GetIdentityType();
            this.issuer = issuer;
            this.email = email;
        }
    }
}