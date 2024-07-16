using Newtonsoft.Json;

namespace Sequence.EmbeddedWallet
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

        public IntentDataFederateAccount(IntentDataOpenSession openSessionIntent, string wallet)
        {
            this.sessionId = openSessionIntent.sessionId;
            this.wallet = wallet;
            this.identityType = openSessionIntent.identityType;
            this.verifier = openSessionIntent.verifier;
            this.answer = openSessionIntent.answer;
        }
    }
}