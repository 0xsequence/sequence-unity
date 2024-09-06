using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class RequestParams<T>
    {
        [JsonProperty("params")]
        public T Params;

        public RequestParams(T Params)
        {
            this.Params = Params;
        }
    }
}