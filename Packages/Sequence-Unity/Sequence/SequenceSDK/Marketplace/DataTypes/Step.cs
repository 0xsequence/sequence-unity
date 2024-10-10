using System;
using Newtonsoft.Json;

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