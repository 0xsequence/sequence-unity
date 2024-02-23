using System;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace Sequence.Provider
{
    public class HttpRpcClient : IRpcClient
    {
        private readonly string _url;
        public HttpRpcClient(string url)
        {
            _url = url;
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
            return await SendRequest(rpcRequestJson);
        }

        public async Task<RpcResponse> SendRequest(string method, object[] parameters)
        {
            RpcRequest rpcRequest = new RpcRequest(BigInteger.One, method, parameters);
            return await SendRequest(rpcRequest);
        }

        public async Task<RpcResponse> SendRequest(string requestJson)
        {
            using var request = UnityWebRequest.Put(_url, requestJson);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("X-Access-Key", "YfeuczOMRyP7fpr1v7h8SvrCAAAAAAAAA"); // Todo: temporary access key while we wait for prod env deployment. Currently, we are using the staging env and we don't have a staging env for node gateway that we can hit publicly);
            request.method = UnityWebRequest.kHttpVerbPOST;
            
            string curlRequest = $"curl -X {request.method} '{_url}' -H 'Content-Type: {request.GetRequestHeader("Content-Type")}' -H 'Accept: {request.GetRequestHeader("Accept")}' -H 'X-Access-Key: {request.GetRequestHeader("X-Access-Key")}' -d '{requestJson}'";

            try
            {
                await request.SendWebRequest();
            
                if (request.error != null || request.result != UnityWebRequest.Result.Success || request.responseCode < 200 || request.responseCode > 299)
                {
                    throw new Exception($"Error sending request to {_url}: {request.responseCode} {request.error}");
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    request.Dispose();
                    var responseJson = Encoding.UTF8.GetString(results);
                    try
                    {
                        RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                        return result;
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error unmarshalling response from {_url}: {e.Message} | given: {responseJson}");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("HTTP Request failed: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data)  + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                throw new Exception("Invalid URL format: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                throw new Exception("File load exception: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                throw new Exception("An unexpected error occurred: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
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


