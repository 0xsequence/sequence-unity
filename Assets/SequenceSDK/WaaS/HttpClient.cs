using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sequence.Provider;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.WaaS
{
    public class HttpClient
    {
        private readonly string _url = "https://next-api.sequence.app/rpc/Wallet";
        private Dictionary<string, string> _defaultHeaders;

        public HttpClient()
        {
            this._defaultHeaders = new Dictionary<string, string>();
        }

        public void AddDefaultHeader(string key, string value)
        {
            this._defaultHeaders[key] = value;
        }

        public async Task<T2> SendRequest<T, T2>(string path, T args, [CanBeNull] Dictionary<string, string> headers = null)
        {
            string url = _url + "/" + path;
            string requestJson = JsonConvert.SerializeObject(args);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.method = UnityWebRequest.kHttpVerbPOST;
            byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";

            if (headers == null)
            {
                headers = _defaultHeaders;
            }
            
            foreach (string key in headers.Keys)
            {
                request.SetRequestHeader(key, headers[key]);
            }

            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            
            if (request.error != null || request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Error sending request to {url} with args {args}: {request.error}");
            }
            else
            {
                byte[] results = request.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                T2 result = JsonConvert.DeserializeObject<T2>(responseJson);
                request.Dispose();
                return result;
            }
        }
        

        private string ExtractHeaders(UnityWebRequest request)
        {
            StringBuilder headerBuilder = new StringBuilder();
            foreach (string headerKey in new string[]{"Content-Type", "Accept", "Authorization"})
            {
                string headerValue = request.GetRequestHeader(headerKey);
                headerBuilder.Append($"-H '{headerKey}: {headerValue}' ");
            }
            return headerBuilder.ToString();
        }
    }
}