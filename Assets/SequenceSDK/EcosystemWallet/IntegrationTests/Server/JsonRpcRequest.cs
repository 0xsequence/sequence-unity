using System;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.IntegrationTests.Server
{
    [Serializable]
    internal class JsonRpcRequest
    {
        public string jsonrpc;
        public string method;
        public object @params;
        public string id;

        [Preserve]
        public JsonRpcRequest(string jsonrpc, string method, object @params, string id)
        {
            this.jsonrpc = jsonrpc;
            this.method = method;
            this.@params = @params;
            this.id = id;
        }
    }
}