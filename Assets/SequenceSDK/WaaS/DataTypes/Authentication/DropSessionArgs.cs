using System;
using Newtonsoft.Json;

namespace SequenceSDK.WaaS.Authentication
{
    [Serializable]
    public class DropSessionArgs
    {
        public string sessionId { get; private set; }
        public string dropSessionId { get; private set; }
        
        public DropSessionArgs(string sessionId, string dropSessionId)
        {
            this.sessionId = sessionId;
            this.dropSessionId = dropSessionId;
        }
    }
}