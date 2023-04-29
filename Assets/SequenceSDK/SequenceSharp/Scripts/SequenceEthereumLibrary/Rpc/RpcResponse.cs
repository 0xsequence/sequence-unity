
namespace SequenceSharp.RPC
{
    [System.Serializable]
    public class RpcResponse 
    {
        public string jsonrpc;
        public int id;
        public string result;
        public RpcError error;

    }
}
