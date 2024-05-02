using System;
using Newtonsoft.Json.Linq;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentResponse<T>
    {
        public Response<T> response;

        public IntentResponse(Response<T> response)
        {
            this.response = response;
        }
    }
    
    public class Response<T>
    {
        public string code;
        public T data;
        
        public Response(string code, T data)
        {
            this.code = code;
            this.data = data;
        }
    }
}