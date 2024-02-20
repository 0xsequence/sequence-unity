using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class WonkyIntentPayload
    {
        public JObject Data { get; private set; }
        public JObject data { get; private set; }
        public uint expiresAt { get; private set; }
        public uint issuedAt { get; private set; }
        public string name { get; private set; }
        public Signature[] signatures { get; private set; }
        public string version { get; private set; }

        [JsonConstructor]
        public WonkyIntentPayload(JObject Data, JObject data, uint expiresAt, uint issuedAt, string name, Signature[] signatures, string version)
        {
            this.Data = Data;
            this.data = data;
            this.expiresAt = expiresAt;
            this.issuedAt = issuedAt;
            this.name = name;
            this.signatures = signatures;
            this.version = version;
        }

        public WonkyIntentPayload(IntentPayload intentPayload)
        {
            this.Data = intentPayload.data;
            this.data = intentPayload.data;
            this.expiresAt = intentPayload.expiresAt;
            this.issuedAt = intentPayload.issuedAt;
            this.name = intentPayload.name;
            this.signatures = intentPayload.signatures;
            this.version = intentPayload.version;
        }
    }
}