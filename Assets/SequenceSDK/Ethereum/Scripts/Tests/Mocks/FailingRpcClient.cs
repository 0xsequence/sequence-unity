using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Wallet;
using UnityEngine;
using Sequence.Provider;

namespace Sequence.Mocks {
    /// <summary>
    /// A mock implementation of IRpcClient that always returns an error
    /// </summary>
    public class FailingRpcClient : IRpcClient
    {

        public static string ErrorMessage = "RPC Error";
        RpcResponse errorResponse = new RpcResponse()
        {
            error = new RpcError()
            {
                Code = -1,
                Message = ErrorMessage
            }
        };

        public async Task<RpcResponse> SendRequest(RpcRequest rpcRequest)
        {
            return errorResponse;
        }

        public async Task<RpcResponse> SendRequest(string method, object[] parameters)
        {
            return errorResponse;
        }

        public async Task<RpcResponse> SendRequest(string requestJson)
        {
            return errorResponse;
        }
    }
}