using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataFinishValidateSession
    {
        public string sessionId;
        public string wallet;
        public string salt;
        public string challenge;

        public IntentDataFinishValidateSession(string sessionId, string wallet, string salt, string challenge)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.salt = salt;
            this.challenge = challenge;
        }
    }
}