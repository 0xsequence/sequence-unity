using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataFinishValidateSession
    {
        public string sessionId { get; private set; }
        public string wallet { get; private set; }
        public string salt { get; private set; }
        public string challenge { get; private set; }

        public IntentDataFinishValidateSession(string sessionId, string wallet, string salt, string challenge)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.salt = salt;
            this.challenge = challenge;
        }
    }
}