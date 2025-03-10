using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sequence.Config;
using Sequence.Utils;
using Sequence.WaaS.DataTypes;

namespace Sequence.EmbeddedWallet
{
    public class HttpClient : IHttpClient
    {
        private readonly string _url;
        private Dictionary<string, string> _defaultHeaders;
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private ResponseSignatureValidator _signatureValidator;
        private string _waasUrl;

        public HttpClient(string url)
        {
            _signatureValidator = new ResponseSignatureValidator();
            _url = url;
            this._defaultHeaders = new Dictionary<string, string>();
            _defaultHeaders["Content-Type"] = "application/json";
            _defaultHeaders["Accept"] = "application/json";
            ISequenceConfig config = SequenceConfig.GetConfig(SequenceService.WaaS);
            _defaultHeaders["X-Access-Key"] = config.BuilderAPIKey;
            _defaultHeaders["Accept-Signature"] = "sig=()";
            if (string.IsNullOrWhiteSpace(config.BuilderAPIKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
            
            ConfigJwt configJwt = SequenceConfig.GetConfigJwt();
            string rpcUrl = configJwt.rpcServer;
            if (string.IsNullOrWhiteSpace(rpcUrl))
            {
                throw SequenceConfig.MissingConfigError("RPC Server");
            }
            _waasUrl = rpcUrl;
        }

        public void AddDefaultHeader(string key, string value)
        {
            this._defaultHeaders[key] = value;
        }

        public (IWebRequest, string, string) BuildRequest<T>(string path, T args,
            [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null)
        {
            string url = _url.AppendTrailingSlashIfNeeded() + path;
            url = url.RemoveTrailingSlash();
            if (overrideUrl != null)
            {
                url = overrideUrl.AppendTrailingSlashIfNeeded() + path;
            }

            IWebRequest request = WebRequestBuilder.Post(url);
            SetHeaders(request, headers);
            
            string requestJson = SetRequestData(request, args);
            
            string method = request.Method;
            string headersString = ExtractHeaders(request);
            string curlRequest = $"curl -X {method} '{url}' {headersString} -d '{requestJson}'";
            if (requestJson == "")
            {
                curlRequest = $"curl -X {method} '{url}' {headersString}";
            }

            return (request, curlRequest, url);
        }

        private void SetHeaders(IWebRequest request, Dictionary<string, string> headers)
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

        private string SetRequestData<T>(IWebRequest request, T args)
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
            request.SetRequestData(requestData);
            return requestJson;
        }

        public async Task<T2> SendRequest<T, T2>(string path, T args, [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null)
        {
            (IWebRequest, string, string) newRequest = BuildRequest(path, args, headers, overrideUrl);
            IWebRequest request = newRequest.Item1;
            string curlRequest = newRequest.Item2;
            string url = newRequest.Item3;
            
            LogHandler.Info(curlRequest);

            try
            {
                var response = await request.Send();

                if (response.Result != WebRequestResult.Success ||
                    response.ResponseCode < 200 || response.ResponseCode > 299)
                {
                    throw new Exception($"Error sending request to {url}: {response.ResponseCode} {request.Error}");
                }
                else
                {
                    while (!_signatureValidator.PublicKeyFetched)
                    {
                        await Task.Yield();
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
                        byte[] results = response.Data;
                        responseJson = Encoding.UTF8.GetString(results);
                    }
                    try
                    {
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
                        throw new TimeMismatchException(exceptionMessage, currentTimeAccordingToServer);
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

        public async Task<TimeSpan> GetTimeShift()
        {
            IWebRequest request = WebRequestBuilder.Get(_waasUrl.AppendTrailingSlashIfNeeded() + "status");

            try
            {
                await request.Send();
                DateTime serverTime = DateTime.Parse(request.GetResponseHeader("date")).ToUniversalTime();
                DateTime localTime = DateTime.UtcNow;
                TimeSpan timeShift = serverTime - localTime;
                return timeShift;
            }
            catch (Exception e)
            {
                LogHandler.Error("Error getting time shift: " + e.Message);
                return TimeSpan.Zero;
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
                return " " + Encoding.UTF8.GetString(request.Data);
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