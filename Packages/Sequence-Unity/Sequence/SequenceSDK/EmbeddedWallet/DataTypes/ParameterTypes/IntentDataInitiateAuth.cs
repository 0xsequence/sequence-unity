using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class IntentDataInitiateAuth
    {
        public string identityType;
        public string metadata;
        public string sessionId;
        public string verifier;

        [Preserve]
        [JsonConstructor]
        public IntentDataInitiateAuth(string identityType, string metadata, string sessionId, string verifier)
        {
            this.identityType = identityType;
            this.metadata = metadata;
            this.sessionId = sessionId;
            this.verifier = verifier;
        }
        
        public IntentDataInitiateAuth(IdentityType identityType, string sessionId, string verifier, string metadata = "")
        {
            this.identityType = identityType.ToString();
            this.sessionId = sessionId;
            this.verifier = verifier;
            this.metadata = metadata;
        }
    }
}