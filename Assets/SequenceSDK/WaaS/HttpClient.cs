using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.Provider;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.WaaS
{
    public class HttpClient
    {
        private readonly string _url = "https://next-api.sequence.app/rpc/Wallet";
        private Dictionary<string, string> _defaultHeaders;
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public HttpClient(string url)
        {
            _url = url;
            this._defaultHeaders = new Dictionary<string, string>();
            _defaultHeaders["Content-Type"] = "application/json";
            _defaultHeaders["Accept"] = "application/json";
            SequenceConfig config = SequenceConfig.GetConfig();
            _defaultHeaders["X-Access-Token"] = config.BuilderAPIKey;
            if (string.IsNullOrWhiteSpace(config.BuilderAPIKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
        }

        public void AddDefaultHeader(string key, string value)
        {
            this._defaultHeaders[key] = value;
        }

        public (UnityWebRequest, string, string) BuildRequest<T>(string path, T args,
            [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null)
        {
            string url = _url + "/" + path;
            if (overrideUrl != null)
            {
                url = overrideUrl.AppendTrailingSlashIfNeeded() + path;
            }
            string requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.method = UnityWebRequest.kHttpVerbPOST;
            byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";

            if (headers == null)
            {
                headers = _defaultHeaders;
            }
            else
            {
                foreach (string key in _defaultHeaders.Keys)
                {
                    if (!headers.ContainsKey(key))
                    {
                        headers[key] = _defaultHeaders[key];
                    }
                }
            }
            
            foreach (string key in headers.Keys)
            {
                request.SetRequestHeader(key, headers[key]);
            }
            
            string method = request.method;
            string headersString = ExtractHeaders(request);
            string curlRequest = $"curl -X {method} '{url}' {headersString} -d '{requestJson}'";

            return (request, curlRequest, url);
        }

        public async Task<T2> SendRequest<T, T2>(string path, T args, [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null)
        {
            (UnityWebRequest, string, string) newRequest = BuildRequest(path, args, headers, overrideUrl);
            UnityWebRequest request = newRequest.Item1;
            string curlRequest = newRequest.Item2;
            string url = newRequest.Item3;

            try
            {
                await request.SendWebRequest();
            
                if (request.error != null || request.result != UnityWebRequest.Result.Success || request.responseCode < 200 || request.responseCode > 299)
                {
                    Debug.LogError($"Error sending request to {url}: {request.responseCode} {request.error}");
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    request.Dispose();
                    var responseJson = Encoding.UTF8.GetString(results);
                    try
                    {
                        T2 result = JsonConvert.DeserializeObject<T2>(responseJson);
                        return result;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error unmarshalling response from {url}: {e.Message} | given: {responseJson}");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("HTTP Request failed: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data)  + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                Debug.LogError("Invalid URL format: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                Debug.LogError("File load exception: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                Debug.LogError("An unexpected error occurred: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl-equivalent request: " + curlRequest);
            }

            return default;
        }

        private string ExtractHeaders(UnityWebRequest request)
        {
            StringBuilder headerBuilder = new StringBuilder();
            foreach (string headerKey in new string[]{"Content-Type", "Accept", "Authorization", "X-Sequence-Tenant", "X-Access-Token"})
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