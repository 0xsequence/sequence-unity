using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class HttpClient : IHttpClient
    {
        private string _apiKey;
        private const string _prodUrl = "https://marketplace-api.sequence.app/";
        private const string _devUrl = "https://dev-marketplace-api.sequence-dev.app/";
        private static string _baseUrl = "https://marketplace-api.sequence.app/";
        private const string _endUrl = "/rpc/Marketplace/";
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        
        public HttpClient()
        {
            ISequenceConfig config = SequenceConfig.GetConfig(SequenceService.Marketplace);
            _apiKey = config.BuilderAPIKey;
#if SEQUENCE_DEV_MARKETPLACE || SEQUENCE_DEV
            _baseUrl = _devUrl;
#else
            _baseUrl = _prodUrl;
#endif            
        }

        public static HttpClient UseHttpClientWithDevEnvironment(string devApiKey)
        {
            _baseUrl = "https://dev-marketplace-api.sequence-dev.app/";
            return new HttpClient(devApiKey);
        }
        
        private HttpClient(string apiKey)
        {
            _apiKey = apiKey;
        }
        
        public async Task<ReturnType> SendRequest<ReturnType>(Chain chain, string url)
        {
            return await SendRequest<object, ReturnType>(chain, url, null);
        }

        public async Task<ReturnType> SendRequest<ArgType, ReturnType>(Chain chain, string endpoint, ArgType args)
        {
            string url = _baseUrl + ChainDictionaries.PathOf[chain] + _endUrl + endpoint;
            return await SendRequest<ArgType, ReturnType>(url, args);
        }

        public async Task<ReturnType> SendRequest<ArgType, ReturnType>(string url, ArgType args)
        {
            string requestJson = "";
            if (args != null)
            {
                requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            }
            using IWebRequest request = WebRequestBuilder.Post(url);
            byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
            request.SetRequestData(requestData);
            request.SetRequestHeader("X-Access-Key", _apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
            string headersString = ExtractHeaders(request);
            string method = request.Method;
            string curlRequest = $"curl -X {method} '{url}' {headersString} -d '{requestJson}'";

            LogHandler.Info(curlRequest);

            try
            {
                var response = await request.Send();

                if (request.Error != null || response.Result != WebRequestResult.Success ||
                    response.ResponseCode < 200 || response.ResponseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {response.ResponseCode} {request.Error}");
                }
                else
                {
                    byte[] results = request.Data;
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

        private string GetRequestErrorIfAvailable(IWebRequest request)
        {
            if (request.Data != null)
            {
                return " reason: " + Encoding.UTF8.GetString(request.Data);
            }

            return "";
        }

        private string ExtractHeaders(IWebRequest request)
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