using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Step
    {
        public StepType id;
        public string data;
        public string to;
        public string value;
        public Signature signature;
        public PostRequest post;

        [Preserve]
        public Step(StepType id, string data, string to, string value, Signature signature, PostRequest post)
        {
            this.id = id;
            this.data = data;
            this.to = to;
            this.value = value;
            this.signature = signature;
            this.post = post;
        }
    }
}