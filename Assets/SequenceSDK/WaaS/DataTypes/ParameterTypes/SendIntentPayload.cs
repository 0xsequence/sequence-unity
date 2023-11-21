using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.WaaS
{
    [Serializable]
    public class SendIntentPayload
    {
        public string sessionId { get; private set; }
        public string intentJson { get; private set; }

        public SendIntentPayload(string sessionId, string intentJson)
        {
            this.sessionId = sessionId;
            this.intentJson = intentJson;
        }  
    }
}