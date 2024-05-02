using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataValidateSession
    {
        public string deviceMetadata;
        public string sessionId;
        public string wallet;

        public IntentDataValidateSession(string sessionId, string wallet, string deviceMetadata)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.deviceMetadata = deviceMetadata;
        }
    }
}