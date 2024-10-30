using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class RequestParams<T>
    {
        [Preserve]
        [JsonProperty("params")]
        public T Params;

        [Preserve]
        public RequestParams(T Params)
        {
            this.Params = Params;
        }
    }
}