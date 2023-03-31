using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace SequenceSharp.RPC
{

    public class HttpRpcClient
    {
        private readonly string _url;
        public HttpRpcClient(string url)
        {
            _url = url;
            Debug.Log("http rpc client");
        }

        public async Task<RpcResponse> SendRequest(RpcRequest rpcRequest)
        {

            var request = new
            {
                jsonrpc = "2.0",
                id = rpcRequest.id,
                method = rpcRequest.method,
                @params = rpcRequest.rawParameters

            };

            var rpcRequestJson = JsonConvert.SerializeObject(request);
            var unityRequest = UnityWebRequest.Put(_url, rpcRequestJson);
            unityRequest.SetRequestHeader("Content-Type", "application/json");
            unityRequest.SetRequestHeader("Accept", "application/json");
            unityRequest.method = UnityWebRequest.kHttpVerbPOST;
            await unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
                unityRequest.Dispose();
                return default;
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                unityRequest.Dispose();
                return result;
            }

        }

        public async Task<RpcResponse> SendRequest(string method, object[] parameters)
        {
            
            var request = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = method,
                @params = parameters
                
            };

            var rpcRequestJson = JsonConvert.SerializeObject(request);

            var unityRequest = UnityWebRequest.Put(_url, rpcRequestJson);

            unityRequest.SetRequestHeader("Content-Type", "application/json");
            unityRequest.SetRequestHeader("Accept", "application/json");
            unityRequest.method = UnityWebRequest.kHttpVerbPOST;
            await unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
                unityRequest.Dispose();
                return default;
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                unityRequest.Dispose();
                return result;
            }

            
        }

        public async Task<RpcResponse> SendRequest(string requestJson)
        {


            var unityRequest = UnityWebRequest.Put(_url, requestJson);

            unityRequest.SetRequestHeader("Content-Type", "application/json");
            unityRequest.SetRequestHeader("Accept", "application/json");
            unityRequest.method = UnityWebRequest.kHttpVerbPOST;
            await unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
                unityRequest.Dispose();
                return default;
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                unityRequest.Dispose();
                return result;
            }


        }

    }

    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
        public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation webReqOp)
        {
            var tcs = new TaskCompletionSource<object>();
            webReqOp.completed += obj =>
            {
                {
                    if (webReqOp.webRequest.responseCode != 200)
                    {
                        tcs.SetException(new FileLoadException(webReqOp.webRequest.error));
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                }
            };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}


