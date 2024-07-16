using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseSessionOpened
    {
        public string sessionId;
        public string wallet;

        public IntentResponseSessionOpened(string sessionId, string wallet)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
        }
    }
}