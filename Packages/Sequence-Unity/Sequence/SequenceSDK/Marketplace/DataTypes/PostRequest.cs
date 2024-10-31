using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [System.Serializable]
    public class PostRequest
    {
        public string endpoint;
        public string method;
        public object body;
        
        [Preserve]
        public PostRequest(string endpoint, string method, object body)
        {
            this.endpoint = endpoint;
            this.method = method;
            this.body = body;
        }
    }
}