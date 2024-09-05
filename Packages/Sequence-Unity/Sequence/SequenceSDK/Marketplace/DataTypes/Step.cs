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
        
        [JsonConstructor]
        public Step(uint id, string data, string to, string value, Signature signature, PostRequest post)
        {
            this.id = (StepType)id;
            this.data = data;
            this.to = to;
            this.value = value;
            this.signature = signature;
            this.post = post;
        }
    }

    public enum StepType
    {
        unknown,
        tokenApproval,
        buy,
        sell,
        createListing,
        createOffer,
        signEIP712,
        signEIP191,
    }
}