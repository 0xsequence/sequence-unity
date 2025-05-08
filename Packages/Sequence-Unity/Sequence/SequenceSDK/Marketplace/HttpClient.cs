using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Config;
using Sequence.Utils;
using UnityEngine.Networking;

namespace Sequence.Marketplace
{
    public class HttpClient : IHttpClient
    {
        private string _apiKey;
        private const string _prodUrl = "https://marketplace-api.sequence.app/";
        private const string _devUrl = "https://dev-marketplace-api.sequence-dev.app/";
        private static string _baseUrl = "https://marketplace-api.sequence.app/";
        private const string _endUrl = "rpc/Marketplace/";
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        
        public HttpClient()
        {
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.Marketplace);
            _apiKey = config.BuilderAPIKey;
#if SEQUENCE_DEV_MARKETPLACE || SEQUENCE_DEV
            _baseUrl = _devUrl;
#else
            _baseUrl = _prodUrl;
#endif            
        }
        
        public async Task<ReturnType> SendRequest<ReturnType>(Chain chain, string url)
        {
            return await SendRequest<object, ReturnType>(chain, url, null);
        }

        public async Task<ReturnType> SendRequest<ArgType, ReturnType>(Chain chain, string endpoint, ArgType args)
        {
            string url = _baseUrl + _endUrl + endpoint;
            string requestJson = ToJson(args);
            if (string.IsNullOrWhiteSpace(requestJson))
            {
                requestJson = "{}";
            }
            string chainId = ChainDictionaries.ChainIdOf[chain];
            JObject jsonObj = JsonConvert.DeserializeObject<JObject>(requestJson);
            jsonObj["chainId"] = chainId;
            requestJson = jsonObj.ToString(Formatting.None);
            
            return await SendRequest<ReturnType>(url, requestJson);
        }

        private string ToJson<ArgType>(ArgType args)
        {
            string requestJson = "";
            if (args != null)
            {
                requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            }

            return requestJson;
        }

        public async Task<ReturnType> SendRequest<ArgType, ReturnType>(string url, ArgType args)
        {
            string requestJson = ToJson(args);

            return await SendRequest<ReturnType>(url, requestJson);
        }

        private async Task<ReturnType> SendRequest<ReturnType>(string url, string requestJson)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.method = UnityWebRequest.kHttpVerbPOST;
            byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("X-Access-Key", _apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
            string headersString = ExtractHeaders(request);
            string method = request.method;
            string curlRequest = $"curl -X {method} '{url}' {headersString} -d '{requestJson}'";

            UnityEngine.Debug.Log(curlRequest);

            try
            {
                await request.SendWebRequest();

                if (request.error != null || request.result != UnityWebRequest.Result.Success ||
                    request.responseCode < 200 || request.responseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {request.responseCode} {request.error}");
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    var responseJson = Encoding.UTF8.GetString(results);
                    try
                    {
                        ReturnType result = JsonConvert.DeserializeObject<ReturnType>(responseJson);
                        return result;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            $"Error unmarshalling response from {url}: {e.Message} | given: {responseJson}");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("HTTP Request failed: " + e.Message + GetRequestErrorIfAvailable(request) +
                                    "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                throw new Exception("Invalid URL format: " + e.Message + GetRequestErrorIfAvailable(request) +
                                    "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                string errorReason = GetRequestErrorIfAvailable(request);
                string exceptionMessage = "File load exception: " + e.Message + " reason: " + errorReason +
                                          "\nCurl-equivalent request: " + curlRequest;
                throw new Exception(exceptionMessage);
            }
            catch (Exception e)
            {
                throw new Exception("An unexpected error occurred: " + e.Message + GetRequestErrorIfAvailable(request) +
                                    "\nCurl-equivalent request: " + curlRequest);
            }
            finally
            {
                request.Dispose();
            }
        }

        private string GetRequestErrorIfAvailable(UnityWebRequest request)
        {
            if (request.downloadHandler != null && request.downloadHandler.data != null)
            {
                return " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data);
            }

            return "";
        }

        private string ExtractHeaders(UnityWebRequest request)
        {
            StringBuilder headerBuilder = new StringBuilder();
            foreach (string headerKey in new string[]{"Content-Type", "Accept", "Authorization", "X-Sequence-Tenant", "X-Access-Key"})
            {
                string headerValue = request.GetRequestHeader(headerKey);
                if (string.IsNullOrEmpty(headerValue))
                {
                    continue;
                }
                headerBuilder.Append($"-H '{headerKey}: {headerValue}' ");
            }
            return headerBuilder.ToString();
        }
    }
}
