using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponse<T>
    {
        public Response<T> response;

        [Preserve]
        public IntentResponse(Response<T> response)
        {
            this.response = response;
        }
    }

    [Preserve]
    [Serializable]
    public class Response<T>
    {
        public string code;
        public T data;
        
        [Preserve]
        public Response(string code, T data)
        {
            this.code = code;
            this.data = data;
        }
    }
}