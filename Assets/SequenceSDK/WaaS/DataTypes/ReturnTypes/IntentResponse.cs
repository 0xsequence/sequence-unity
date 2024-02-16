using System;
using Newtonsoft.Json.Linq;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentResponse<T>
    {
        public string code { get; private set; }
        public T data { get; private set; }
        
        public IntentResponse(string code, T data)
        {
            this.code = code;
            this.data = data;
        }
    }
}