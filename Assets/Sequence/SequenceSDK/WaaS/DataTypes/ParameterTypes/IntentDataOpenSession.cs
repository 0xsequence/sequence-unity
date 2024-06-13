using System;
using Newtonsoft.Json;
using Sequence;
using Sequence.Utils;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataOpenSession
    {
        public string email { get; private set; }
        public string idToken { get; private set; }
        public string sessionId { get; private set; }

        public IntentDataOpenSession(Address sessionWallet, string email = null, string idToken = null)
        {
            this.sessionId = CreateSessionId(sessionWallet);
            this.email = email;
            this.idToken = idToken;
        }
        
        [JsonConstructor]
        public IntentDataOpenSession(string sessionId, string email, string idToken)
        {
            this.sessionId = sessionId;
            this.email = email;
            this.idToken = idToken;
        }
        
        public static string CreateSessionId(Address sessionWallet)
        {
            return $"0x00{sessionWallet.Value.ToLower().WithoutHexPrefix()}";
        }
    }
}