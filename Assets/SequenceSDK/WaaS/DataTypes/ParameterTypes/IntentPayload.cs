using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentPayload
    {
        public string version { get; private set; }
        public JObject packet { get; private set; }
        public Signature[] signatures { get; private set; }
        
        public IntentPayload(string version, JObject packet, Signature[] signatures)
        {
            this.version = version;
            this.packet = packet;
            this.signatures = signatures;
        }

        public IntentPayload(string version, JObject packet, params (string, string)[] signatures)
        {
            this.version = version;
            this.packet = packet;
            this.signatures = new Signature[signatures.Length];
            int length = signatures.Length;
            for (int i = 0; i < length; i++)
            {
                this.signatures[i] = new Signature(signatures[i].Item1, signatures[i].Item2);
            }
        }
    }
}