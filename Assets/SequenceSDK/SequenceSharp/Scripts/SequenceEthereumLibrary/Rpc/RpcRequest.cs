
using System.Numerics;

namespace Sequence.RPC
{
    [System.Serializable]
    public class RpcRequest
    {
        public string jsonrpc = "2.0";
        public BigInteger id;
        public string method;
        public object[] rawParameters;
        public RpcRequest(BigInteger _id, string _method, params object[] _parameterList)
        {
            id = _id;
            method = _method;
            rawParameters = _parameterList;
        }

        
    }
}