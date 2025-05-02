using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet 
{
    [Serializable]
    [Preserve]
    public class Account
    {
        public string email;
        public string id;
        public IdentityType identityType;
        public string issuer;
        public Address wallet; // This is not returned from the API - instead, we set it to the wallet address we are federating against when making the request
        
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