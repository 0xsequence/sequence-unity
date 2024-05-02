using System;
using Newtonsoft.Json;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentResponseSessionOpened
    {
        public string sessionId;
        public string wallet;

        [JsonConstructor]
        public IntentResponseSessionOpened(string sessionId, string wallet)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
        }
    }
}