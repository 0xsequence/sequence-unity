using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence
{
    public class HttpHandler : IHttpHandler
    {
        private static List<UnityWebRequest> _streams = new();
        
        private string _builderApiKey;
        private IIndexer _caller;

        public HttpHandler(string builderApiKey, IIndexer caller = null)
        {
            _builderApiKey = builderApiKey;
            _caller = caller;
        }

        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        
        public async Task<string> HttpPost(string chainID, string endPoint, object args, int retries = 0)
        {
            string requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            using var req = UnityWebRequest.Put(Url(chainID, endPoint), requestJson);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.SetRequestHeader("X-Access-Key", _builderApiKey); 
            req.method = UnityWebRequest.kHttpVerbPOST;
            req.timeout = 10; // Request will timeout after 10 seconds
            
            string curlRequest = 
                $"curl -X POST -H \"Content-Type: application/json\" -H \"Accept: application/json\" -H \"X-Access-Key: {req.GetRequestHeader("X-Access-Key")}\" -d '{requestJson}' {Url(chainID, endPoint)}";
            try
            {
                await req.SendWebRequest();
                if (req.responseCode < 200 || req.responseCode > 299 || req.error != null ||
                    req.result == UnityWebRequest.Result.ConnectionError ||
                    req.result == UnityWebRequest.Result.ProtocolError)
                {
                    throw new Exception("Failed to make request, non-200 status code " + req.responseCode +
                                        " with error: " + req.error + "\nCurl-equivalent request: " + curlRequest);
                }

                string returnText = req.downloadHandler.text;
                req.Dispose();
                return returnText;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("HTTP Request failed: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
               throw new FormatException("Invalid URL format: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                if (e.Message.Contains($"{(int)HttpStatusCode.TooManyRequests}"))
                {
                    if (retries == 5)
                    {
                        throw new Exception("Sequence server rate limit exceeded, giving up after 5 retries..." + "\nCurl-equivalent request: " + curlRequest);
                    }
                    else
                    {
                        string issue =
                            $"Sequence server rate limit exceeded, trying again... Retries so far: {retries}" +
                            "\nCurl-equivalent request: " + curlRequest;
                        Indexer.OnQueryIssue?.Invoke(issue);
                        if (_caller != null)
                        {
                            _caller.OnQueryEncounteredAnIssue(issue);
                        }
                        return await RetryHttpPost(chainID, endPoint, args, 5 * retries, retries);
                    }
                }   
                else
                {
                    string issue = "File load exception: " + e.Message + "\nCurl-equivalent request: " + curlRequest;
                    Indexer.OnQueryIssue?.Invoke(issue);
                    if (_caller != null)
                    {
                        _caller.OnQueryEncounteredAnIssue(issue);
                    }
                }
            }
            catch (Exception e) {
                throw new Exception("An unexpected error occurred: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            finally
            {
                req.Dispose();
            }

            return "";
        }

        public async Task<T> HttpPost<T>(string url, object args)
        {
            string requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            using var req = UnityWebRequest.Put(url, requestJson);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.SetRequestHeader("X-Access-Key", _builderApiKey); 
            req.method = UnityWebRequest.kHttpVerbPOST;
            req.timeout = 10;
            
            string curlRequest = 
                $"curl -X POST -H \"Content-Type: application/json\" -H \"Accept: application/json\" -H \"X-Access-Key: {req.GetRequestHeader("X-Access-Key")}\" -d '{requestJson}' {url}";
            
            try
            {
                await req.SendWebRequest();
                if (req.responseCode < 200 || req.responseCode > 299 || req.error != null ||
                    req.result == UnityWebRequest.Result.ConnectionError ||
                    req.result == UnityWebRequest.Result.ProtocolError)
                {
                    throw new Exception("Failed to make request, non-200 status code " + req.responseCode +
                                        " with error: " + req.error + "\nCurl-equivalent request: " + curlRequest);
                }

                byte[] results = req.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                try
                {
                    T result = JsonConvert.DeserializeObject<T>(responseJson);
                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception(
                        $"Error unmarshalling response from {url}: {e.Message} | given: {responseJson}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("HTTP Request failed: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
               throw new FormatException("Invalid URL format: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                throw new FileLoadException("File load exception: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (Exception e) {
                throw new Exception("An unexpected error occurred: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            finally
            {
                req.Dispose();
            }
        }

        public async void HttpStream<T>(string chainID, string endPoint, object args, WebRPCStreamOptions<T> options)
        {
            var requestJson = JsonConvert.SerializeObject(args, serializerSettings);
            using var req = UnityWebRequest.Put(Url(chainID, endPoint), requestJson);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("X-Access-Key", _builderApiKey); 
            req.downloadHandler = new DownloadHandlerStream<T>(options);
            req.method = UnityWebRequest.kHttpVerbPOST;
            
            _streams.Add(req);
            await req.SendWebRequest();
            
            if (_streams.Contains(req))
                _streams.Remove(req);
        }

        public void AbortStreams()
        {
            foreach (var stream in _streams)
                stream.Abort();
            
            _streams.Clear();
        }

        private async Task<string> RetryHttpPost(string chainID, string endPoint, object args, float waitInSeconds, int retries)
        {
            await AsyncExtensions.DelayTask(waitInSeconds);
            return await HttpPost(chainID, endPoint, args, retries + 1);
        }
        
        /// <summary>
        /// Combines <see cref="PATH"/> and <paramref name="name"/> to suffix on to the Base Address
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string Url(string chainID, string endPoint)
        {
            return $"{HostName(chainID)}{Indexer.PATH}{endPoint}";
        }
        
        /// <summary>
        /// Get HostName directing to specific <paramref name="chainID"/>
        /// </summary>
        /// <param name="chainID"></param>
        /// <returns></returns>
        /// <exception>Throws if the chainID isn't a Sequence-supported chain.</exception>
        private string HostName(string chainID)
        {
            var indexerName = ChainDictionaries.PathOf[ChainDictionaries.ChainById[chainID]];
#if SEQUENCE_DEV_INDEXER || SEQUENCE_DEV            
            return $"https://dev-{indexerName}-indexer.sequence.app";
#else
            return $"https://{indexerName}-indexer.sequence.app";
#endif            
        }
    }
}
