using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;

namespace Sequence.EcosystemWallet.IntegrationTests.Server
{
    internal class Server
    {
        public static readonly Dictionary<string, Func<Dictionary<string, object>, Task<object>>> Methods =
            new Dictionary<string, Func<Dictionary<string, object>, Task<object>>>
            {
                ["payload_toAbi"] = async (parameters) => await new PayloadTests().PayloadToAbi(parameters),
                ["payload_toPacked"] = async (parameters) => await new PayloadTests().PayloadToPacked(parameters),
                ["payload_toJson"] = async (parameters) => await new PayloadTests().PayloadToJson(parameters),
                ["payload_hashFor"] = async (parameters) => await new PayloadTests().PayloadHashFor(parameters),
                ["config_new"] = async (parameters) => await new ConfigTests().ConfigNew(parameters),
                ["config_encode"] = async (parameters) => await new ConfigTests().ConfigEncode(parameters),
                ["config_imageHash"] = async (parameters) => await new ConfigTests().ConfigImageHash(parameters),
                ["devTools_randomConfig"] = async (parameters) => await new DevToolsTest().DevToolsRandomConfig(parameters),
                ["devTools_randomSessionTopology"] = async (parameters) => await new DevToolsTest().DevToolsRandomConfig(parameters),
            };

        public async Task<JsonRpcResponse> HandleSingleRequest(
            JsonRpcRequest rpcRequest,
            bool debug,
            bool silent)
        {
            var id = rpcRequest.id;
            var jsonrpc = rpcRequest.jsonrpc;
            var method = rpcRequest.method;
            var @params = rpcRequest.@params;

            if (!silent)
            {
                Debug.Log($"[{DateTime.UtcNow:O}] Processing request: method={method} id={id}");
            }

            if (debug && !silent)
            {
                Debug.Log("Request details: " + JsonConvert.SerializeObject(rpcRequest, Formatting.Indented));
            }

            if (jsonrpc != "2.0")
            {
                JsonRpcErrorResponse error =
                    new JsonRpcErrorResponse(new JsonRpcErrorResponse.Error(-32600, "Invalid JSON-RPC version"), id);
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] Error response: {(debug ? JsonConvert.SerializeObject(error) : error.error.message)}");
                }

                return error;
            }
            
            if (!Methods.ContainsKey(method))
            {
                JsonRpcErrorResponse error = new JsonRpcErrorResponse(new JsonRpcErrorResponse.Error(-32601, $"Method not found: {method}"), id);
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] Error response: {(debug ? JsonConvert.SerializeObject(error) : error.error.message)}");
                }
                return error;
            }
            
            try
            {
                Dictionary<string, object> methodParams;
                
                if (debug && !silent)
                {
                    Debug.Log($"[{DateTime.UtcNow:O}] Raw params: {JsonConvert.SerializeObject(@params)}");
                }
                
                // Convert params to JObject for more flexible parsing if needed
                Newtonsoft.Json.Linq.JObject paramsJObject = null;
                if (@params is Dictionary<string, object> paramsDict)
                {
                    paramsJObject = Newtonsoft.Json.Linq.JObject.FromObject(paramsDict);
                }
                else if (@params is Newtonsoft.Json.Linq.JObject jObj)
                {
                    paramsJObject = jObj;
                }
                
                methodParams = paramsJObject?.ToObject<Dictionary<string, object>>();
                
                if (methodParams == null || methodParams.Count == 0)
                {
                    Debug.LogWarning("No method params");
                }
                
                if (debug && !silent)
                {
                    Debug.Log($"[{DateTime.UtcNow:O}] Final methodParams: {JsonConvert.SerializeObject(methodParams)}");
                }
                
                var result = await Methods[method](methodParams);
                JsonRpcSuccessResponse response = new JsonRpcSuccessResponse(result, id);
                if (!silent)
                {
                    Debug.Log($"[{DateTime.UtcNow:O}] Success Response for Method={method} id={id}");
                    if (debug)
                    {
                        Debug.Log("Response details: " + JsonConvert.SerializeObject(response, Formatting.Indented));
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                JsonRpcErrorResponse error = new JsonRpcErrorResponse(new JsonRpcErrorResponse.Error(-32000, $"Unknown error: {ex.Message}"), id);
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] Error response: {(debug ? JsonConvert.SerializeObject(error) : error.error.message)}");
                }
                return error;
            }
        }
        
        // Todo cleanup, lots of repeated code
        public async Task HandleHttpRequest(HttpListenerContext context, bool debug, bool silent)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            
            if (!silent)
            {
                Debug.Log($"[{DateTime.UtcNow:O}] {request.HttpMethod} {request.Url.PathAndQuery} from {request.RemoteEndPoint}");
            }
            
            // Only handle POST /rpc
            if (request.HttpMethod != "POST" || request.Url.PathAndQuery != "/rpc")
            {
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] 404 Not Found");
                }
                response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.Close();
                return;
            }
            
            // Read the request body
            string body;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = await reader.ReadToEndAsync();
            }
            
            if (debug && !silent)
            {
                Debug.Log("Raw request body: " + body);
            }
            
            // Try to parse JSON. If invalid, return an error
            try
            {
                object jsonRpcRequests = JsonConvert.DeserializeObject(body);
                
                response.ContentType = "application/json";
                
                if (jsonRpcRequests is Newtonsoft.Json.Linq.JArray array)
                {
                    if (!silent)
                    {
                        Debug.Log($"[{DateTime.UtcNow:O}] Processing batch request with {array.Count} items");
                    }
                    
                    var batchRequests = array.ToObject<JsonRpcRequest[]>();
                    var results = new List<JsonRpcResponse>();
                    
                    foreach (var req in batchRequests)
                    {
                        results.Add(await HandleSingleRequest(req, debug, silent));
                    }
                    
                    string responseJson = JsonConvert.SerializeObject(results);
                    byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    var singleRequest = JsonConvert.DeserializeObject<JsonRpcRequest>(body);
                    var result = await HandleSingleRequest(singleRequest, debug, silent);
                    
                    string responseJson = JsonConvert.SerializeObject(result);
                    byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (JsonException ex)
            {
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] JSON parse error: {ex.Message}");
                }
                
                response.StatusCode = 400;
                var errorResp = new JsonRpcErrorResponse(
                    new JsonRpcErrorResponse.Error(-32700, "Parse error", ex.Message), 
                    null);
                
                string responseJson = JsonConvert.SerializeObject(errorResp);
                byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] Internal server error: {ex.Message}");
                }
                
                response.StatusCode = 500;
                var errorResp = new JsonRpcErrorResponse(
                    new JsonRpcErrorResponse.Error(-32000, "Internal server error", ex.Message), 
                    null);
                
                string responseJson = JsonConvert.SerializeObject(errorResp);
                byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                response.Close();
            }
        }
        
        public void StartServer(string host, int port, bool debug, bool silent)
        {
            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add($"http://{host}:{port}/");
                listener.Start();
                
                if (!silent)
                {
                    Debug.Log($"[{DateTime.UtcNow:O}] RPC server running at http://{host}:{port}/rpc");
                    if (debug)
                    {
                        Debug.Log("Debug mode enabled - detailed logging active");
                    }
                }
                
                Task.Run(async () =>
                {
                    try
                    {
                        while (listener.IsListening)
                        {
                            var context = await listener.GetContextAsync();
                            
                            // Handle the request in another task to allow the server to continue listening
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await HandleHttpRequest(context, debug, silent);
                                }
                                catch (Exception ex)
                                {
                                    if (!silent)
                                    {
                                        Debug.LogError($"[{DateTime.UtcNow:O}] Error handling request: {ex.Message}");
                                    }
                                    
                                    try
                                    {
                                        HttpListenerResponse response = context.Response;
                                        response.StatusCode = 500;
                                        var errorResp = new JsonRpcErrorResponse(
                                            new JsonRpcErrorResponse.Error(-32000, "Internal server error", ex.Message), 
                                            null);
                                        
                                        string responseJson = JsonConvert.SerializeObject(errorResp);
                                        byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                                        response.ContentLength64 = buffer.Length;
                                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                                        response.Close();
                                    }
                                    catch
                                    {
                                        // Ignore errors when trying to respond with an error
                                    }
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!silent)
                        {
                            Debug.LogError($"[{DateTime.UtcNow:O}] Server listener error: {ex.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    Debug.LogError($"[{DateTime.UtcNow:O}] Failed to start server: {ex.Message}");
                }
            }
        }
        
        [MenuItem("Sequence Dev/Start Wallet V3 Test Server")]
        public static void DoStartServer()
        {
            new Server().StartServer("localhost", 8080, true, false);
        }
    }
}