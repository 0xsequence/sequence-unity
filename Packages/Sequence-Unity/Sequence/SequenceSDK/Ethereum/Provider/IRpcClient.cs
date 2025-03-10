using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.Provider
{
	public interface IRpcClient
	{
        Task<RpcResponse> SendRequest(RpcRequest rpcRequest);
        Task<RpcResponse> SendRequest(string method, object[] parameters);
        Task<RpcResponse> SendRequest(string requestJson);
	}
}