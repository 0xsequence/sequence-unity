using System;
using Newtonsoft.Json.Linq;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentReturn<T>
    {
        public string code { get; private set; }
        public T data { get; private set; }
        
        public IntentReturn(string code, T data)
        {
            this.code = code;
            this.data = data;
        }
    }
}