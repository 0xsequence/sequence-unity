using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.Provider
{
    [Preserve]
    [System.Serializable]
    public class RpcRequest
    {
        public string jsonrpc = "2.0";
        public BigInteger id;
        public string method;
        public object[] @params;
        
        public RpcRequest(BigInteger _id, string _method, params object[] _params)
        {
            id = _id;
            method = _method;
            @params = _params;
        }
    }
}