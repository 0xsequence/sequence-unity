using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Sequence.EcosystemWallet.Authentication
{
    public class EditorServer
    {
        private HttpListener _listener;
        
        public async Task HandleHttpRequest(HttpListenerContext context)
        {
            Debug.Log($"{context.Request.HttpMethod} {context.Request.RawUrl}");

            var urlParams = GetUrlParams(context.Request.RawUrl);
            var id = urlParams["id"];
            var encodedPayload = urlParams["payload"];
            var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPayload));
            var payload = JsonConvert.DeserializeObject<AuthResponse>(payloadJson);
            
            Debug.Log($"Done: {payload.walletAddress}, {payload.email}, {payload.loginMethod}");
        }

        public void StopListening()
        {
            _listener.Stop();
        }

        public void StartServer(string host, int port)
        {
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://{host}:{port}/");
                _listener.Start();
                
                Task.Run(async () =>
                {
                    try
                    {
                        while (_listener.IsListening)
                        {
                            var context = await _listener.GetContextAsync();
                            
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await HandleHttpRequest(context);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError($"[{DateTime.UtcNow:O}] Error handling request: {ex.Message}");
                                    
                                    try
                                    {
                                        HttpListenerResponse response = context.Response;
                                        response.StatusCode = 500;
                                        
                                        string responseJson = "Internal server error";
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
                        Debug.LogError($"[{DateTime.UtcNow:O}] Server listener error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{DateTime.UtcNow:O}] Failed to start server: {ex.Message}");
            }
        }

        private Dictionary<string, string> GetUrlParams(string url)
        {
            var uri = new Uri("http://localhost" + url);
            var query = uri.Query.TrimStart('?');
            var pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);

            var dict = new Dictionary<string, string>();
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=', 2);
                if (kv.Length == 2)
                {
                    var key = Uri.UnescapeDataString(kv[0]);
                    var value = Uri.UnescapeDataString(kv[1]);
                    dict[key] = value;
                }
            }

            return dict;
        }
    }
}