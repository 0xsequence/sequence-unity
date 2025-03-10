using System;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils;

namespace Sequence.Provider
{
    public class HttpRpcClient : IRpcClient
    {
        private readonly string _url;
        private string _builderApiKey;
        public HttpRpcClient(string url)
        {
            _url = url;
            _builderApiKey = SequenceConfig.GetConfig(SequenceService.NodeGateway).BuilderAPIKey;
        }

        public async Task<RpcResponse> SendRequest(RpcRequest rpcRequest)
        {
            var rpcRequestJson = JsonConvert.SerializeObject(rpcRequest);
            return await SendRequest(rpcRequestJson);
        }

        public async Task<RpcResponse> SendRequest(string method, object[] parameters)
        {
            RpcRequest rpcRequest = new RpcRequest(BigInteger.One, method, parameters);
            return await SendRequest(rpcRequest);
        }

        public async Task<RpcResponse> SendRequest(string requestJson)
        {
            if (string.IsNullOrWhiteSpace(_builderApiKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
            
            using var request = WebRequestBuilder.Post(_url, requestJson);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("X-Access-Key", _builderApiKey); 
            
            string curlRequest = $"curl -X {request.Method} '{_url}' -H 'Content-Type: {request.GetRequestHeader("Content-Type")}' -H 'Accept: {request.GetRequestHeader("Accept")}' -H 'X-Access-Key: {request.GetRequestHeader("X-Access-Key")}' -d '{requestJson}'";

            try
            {
                var response = await request.Send();
            
                if (request.Error != null || response.Result != WebRequestResult.Success || response.ResponseCode < 200 || response.ResponseCode > 299)
                {
                    throw new Exception($"Error sending request to {_url}: {response.ResponseCode} {request.Error}");
                }
                else
                {
                    byte[] results = request.Data;
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
                throw new Exception("HTTP Request failed: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data)  + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                throw new Exception("Invalid URL format: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                throw new Exception("File load exception: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                throw new Exception("An unexpected error occurred: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl-equivalent request: " + curlRequest);
            }
            finally
            {
                request.Dispose();
            }
        }
    }
}


