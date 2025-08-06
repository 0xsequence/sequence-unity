using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sequence.Utils
{
    public class HttpClient : IHttpClient
    {
        private readonly string _baseUrl;
        private readonly Dictionary<string, string> _defaultHeaders;
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        
        public HttpClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _defaultHeaders = new Dictionary<string, string>();
            _defaultHeaders["Content-Type"] = "application/json";
            _defaultHeaders["Accept"] = "application/json";
            _defaultHeaders["Accept-Signature"] = "sig=()";
        }
        
        public async Task<TResponse> SendPostRequest<TArgs, TResponse>(string path, TArgs args, Dictionary<string, string> headers = null)
        {
            var newRequest = BuildRequest(path, UnityWebRequest.kHttpVerbPOST, args, headers);
            var request = newRequest.Item1;
            var curlRequest = newRequest.Item2;
            var url = newRequest.Item3;
            
            string requestJson = SetRequestData(request, args);
            
            string method = request.method;
            string headersString = ExtractHeaders(request);
            string curlRequest = $"curl -X {method} '{url}' {headersString} -d '{requestJson}'";
            if (requestJson == "")
            {
                curlRequest = $"curl -X {method} '{url}' {headersString}";
            }

            return (request, curlRequest, url);
        }

        private void SetHeaders(UnityWebRequest request, Dictionary<string, string> headers)
        {
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
                if (headers[key] == null)
                {
                    continue;
                }
                request.SetRequestHeader(key, headers[key]);
            }
        }

        private string SetRequestData<T>(UnityWebRequest request, T args)
        {
            string requestJson = "";
            if (typeof(T) == typeof(string))
            {
                requestJson = args as string;
            }
            else
            {
                requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            }
            byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";
            return requestJson;
        }

        public async Task<T2> SendRequest<T, T2>(string path, T args, [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null)
        {
            (UnityWebRequest, string, string) newRequest = BuildRequest(path, args, headers, overrideUrl);
            UnityWebRequest request = newRequest.Item1;
            string curlRequest = newRequest.Item2;
            string url = newRequest.Item3;
            
            SequenceLog.Info($">> {curlRequest}");

            try
            {
                await request.SendWebRequest();

                if (request.error != null || request.result != UnityWebRequest.Result.Success ||
                    request.responseCode < 200 || request.responseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {request.responseCode} {request.error}");
                }

                    string responseJson = "";
                    if (url.Contains(_waasUrl))
                    {
                        try
                        {
                            responseJson = _signatureValidator.ValidateResponse(request);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error validating response: " + e.Message + " Warning: this response may have been tampered with!");
                        }
                    }
                    else
                    {
                        byte[] results = request.downloadHandler.data;
                        responseJson = Encoding.UTF8.GetString(results);
                    }
                    try
                    {
                        SequenceLog.Info($"<< {responseJson} (from {url})");
                        T2 result = JsonConvert.DeserializeObject<T2>(responseJson);
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
                string exceptionMessage = "File load exception: " + e.Message + " response: " + errorReason +
                                          "\nCurl-equivalent request: " + curlRequest;
                if (errorReason.Contains("intent is invalid: intent expired") || errorReason.Contains("intent is invalid: intent issued in the future"))
                {
                    string dateHeader = request.GetResponseHeader("date");
                    if (string.IsNullOrWhiteSpace(dateHeader))
                    {
                        exceptionMessage += "\nNo date header found in response";
                        throw new Exception(exceptionMessage);
                    }

                    if (DateTime.TryParseExact(dateHeader, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateTime))
                    {
                        long currentTimeAccordingToServer = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
                        throw new Exception($"{exceptionMessage}, {currentTimeAccordingToServer}");
                    }
                    else
                    {
                        exceptionMessage += "\nUnable to parse server time from date header in response";
                        throw new Exception(exceptionMessage);
                    }
                }
                else if (errorReason.Contains("JWT validation: aud not satisfied"))
                {
                    exceptionMessage = "File load exception: " + e.Message + " response: " + errorReason +
                                       " Please make sure you've whitelisted the associated login method and associated configuration values in your Embedded Wallet configuration in the Sequence Builder!" 
                                       + "\nCurl-equivalent request: " + curlRequest;
                }
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
        
        public async Task<TResponse> SendGetRequest<TResponse>(string path, Dictionary<string, string> headers = null)
        {
            var url = _baseUrl.AppendTrailingSlashIfNeeded() + path;
            var request = UnityWebRequest.Get(url);
            request.method = UnityWebRequest.kHttpVerbGET;
            SetHeaders(request, headers);

            try
            {
                await request.SendWebRequest();
                
                if (request.error != null || request.result != UnityWebRequest.Result.Success ||
                    request.responseCode < 200 || request.responseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {request.responseCode} {request.error}");
                }

                var results = request.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                var response = JsonConvert.DeserializeObject<TResponse>(responseJson, _serializerSettings);
                
                return response;
            }
            catch (Exception e)
            {
                SequenceLog.Error("Error getting time shift: " + e.Message);
                return TimeSpan.Zero;
            }
            finally
            {
                request.Dispose();
            }
        }

        public (UnityWebRequest, string, string) BuildRequest<T>(string path, string method, T args,
            [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + path;
            url = url.RemoveTrailingSlash();
            if (overrideUrl != null)
            {
                url = overrideUrl.AppendTrailingSlashIfNeeded() + path;
            }

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.method = method;
            SetHeaders(request, headers);
            
            string requestJson = SetRequestData(request, args);
            
            string headersString = ExtractHeaders(request);
            string curlRequest = $"curl -X {method} '{url}' {headersString} -d '{requestJson}'";
            if (requestJson == "")
            {
                curlRequest = $"curl -X {method} '{url}' {headersString}";
            }

            return (request, curlRequest, url);
        }
        
        private void SetHeaders(UnityWebRequest request, Dictionary<string, string> headers)
        {
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
                if (headers[key] == null)
                {
                    continue;
                }
                request.SetRequestHeader(key, headers[key]);
            }
        }

        private string SetRequestData<T>(UnityWebRequest request, T args)
        {
            string requestJson = "";
            if (typeof(T) == typeof(string))
            {
                requestJson = args as string;
            }
            else
            {
                requestJson = JsonConvert.SerializeObject(args, _serializerSettings);
            }
            byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";
            return requestJson;
        }
        
        private string GetRequestErrorIfAvailable(UnityWebRequest request)
        {
            if (request.downloadHandler != null && request.downloadHandler.data != null)
                return " " + Encoding.UTF8.GetString(request.downloadHandler.data);

            return string.Empty;
        }
        
        private string ExtractHeaders(UnityWebRequest request)
        {
            var headerBuilder = new StringBuilder();
            foreach (string headerKey in new []{"Content-Type", "Accept", "Authorization", "X-Sequence-Tenant", "X-Access-Key"})
            {
                var headerValue = request.GetRequestHeader(headerKey);
                if (string.IsNullOrEmpty(headerValue))
                    continue;
                
                headerBuilder.Append($"-H '{headerKey}: {headerValue}' ");
            }
            return headerBuilder.ToString();
        }
    }
}