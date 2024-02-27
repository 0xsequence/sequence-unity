using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataValidateSession
    {
        public string deviceMetadata { get; private set; }
        public string sessionId { get; private set; }
        public string wallet { get; private set; }

        public IntentDataValidateSession(string sessionId, string wallet, string deviceMetadata)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.deviceMetadata = deviceMetadata;
        }
    }
}