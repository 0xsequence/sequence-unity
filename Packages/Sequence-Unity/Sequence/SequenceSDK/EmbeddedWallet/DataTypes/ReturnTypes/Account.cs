using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet 
{
    [Preserve]
    public class Account
    {
        public string email;
        public string id;
        public IdentityType identityType;
        public string issuer;
        
        [Preserve]
        public Account(string id, string type, string issuer, string email)
        {
            this.id = id;
            this.identityType = type.GetIdentityType();
            this.issuer = issuer;
            this.email = email;
        }
    }
}