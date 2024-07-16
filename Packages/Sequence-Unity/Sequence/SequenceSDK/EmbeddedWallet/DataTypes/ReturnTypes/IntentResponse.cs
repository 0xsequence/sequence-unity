using System;
using Newtonsoft.Json.Linq;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponse<T>
    {
        public Response<T> response { get; private set; }

        public IntentResponse(Response<T> response)
        {
            this.response = response;
        }
    }
    
    public class Response<T>
    {
        public string code { get; private set; }
        public T data { get; private set; }
        
        public Response(string code, T data)
        {
            this.code = code;
            this.data = data;
        }
    }
}