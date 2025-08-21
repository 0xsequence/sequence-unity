using System;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.IntegrationTests.Server
{
    [Serializable]
    internal class JsonRpcErrorResponse : JsonRpcResponse
    {
        public const string jsonrpc = "2.0";
        public Error error;
        public string id;

        [Preserve]
        public JsonRpcErrorResponse(Error error, string id)
        {
            this.error = error;
            this.id = id;
        }

        [Serializable]
        public class Error
        {
            public int code;
            public string message;
            public object data;

            [Preserve]
            public Error(int code, string message, object data = null)
            {
                this.code = code;
                this.message = message;
                this.data = data;
            }
        }

        public override bool IsError()
        {
            return true;
        }
    }
}