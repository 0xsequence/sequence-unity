using System;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.IntegrationTests.Server
{
    [Serializable]
    internal class JsonRpcSuccessResponse : JsonRpcResponse
    {
        public const string jsonrpc = "2.0";
        public object result;
        public string id;

        [Preserve]
        public JsonRpcSuccessResponse(object result, string id)
        {
            this.result = result;
            this.id = id;
        }

        public override bool IsError()
        {
            return false;
        }
    }
}