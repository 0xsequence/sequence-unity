using Newtonsoft.Json;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public class IntentDataFederateAccount
    {
        public string answer;
        public string identityType;
        public string sessionId;
        public string verifier;
        public string wallet;
        
        [JsonConstructor]
        public IntentDataFederateAccount(string sessionId, string wallet, string identityType, string verifier, string answer)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.identityType = identityType;
            this.verifier = verifier;
            this.answer = answer;
        }

        public IntentDataFederateAccount(Address wallet, IdentityType identityType, string verifier, string answer)
        {
            this.wallet = wallet;
            this.identityType = identityType.ToString();
            this.verifier = verifier;
            this.answer = answer;
            this.sessionId = IntentDataOpenSession.CreateSessionId(wallet);
        }
    }
}