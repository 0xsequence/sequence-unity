using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentResponse<T>
    {
        public Response<T> response;

        [UnityEngine.Scripting.Preserve]
        public IntentResponse(Response<T> response)
        {
            this.response = response;
        }
    }

    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class Response<T>
    {
        public string code;
        public T data;
        
        [UnityEngine.Scripting.Preserve]
        public Response(string code, T data)
        {
            this.code = code;
            this.data = data;
        }
    }
}