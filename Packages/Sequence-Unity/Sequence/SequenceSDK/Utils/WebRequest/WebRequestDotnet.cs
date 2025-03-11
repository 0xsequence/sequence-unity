using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sequence.Utils
{
    public class WebRequestDotnet : IWebRequest
    {
        public string Method { get; private set; }
        public string Error { get; private set; }
        public string Text { get; private set; }
        public byte[] Data { get; private set; }

        private readonly System.Net.Http.HttpClient _client;
        private readonly string _url;
        private readonly HttpMethod _method;
        private readonly Dictionary<string, string> _headers;
        private readonly Dictionary<string, string> _responseHeaders;
        private byte[] _data;
        
        public WebRequestDotnet(string url, string method)
        {
            Method = method;
            _client = new System.Net.Http.HttpClient();
            _headers = new Dictionary<string, string>();
            _responseHeaders = new Dictionary<string, string>();
            _url = url;

            _method = method switch
            {
                "POST" => HttpMethod.Post,
                _ => HttpMethod.Get
            };
        }
        
        public void SetRequestData(byte[] data, string contentType = "application/json")
        {
            _data = data;
        }

        public void SetRequestHeader(string key, string value)
        {
            _headers.Add(key, value);
        }

        public void SetTimeout(int timeout)
        {
            
        }

        public string GetRequestHeader(string key)
        {
            return _headers.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public string GetResponseHeader(string key)
        {
            // .NET HttpClient can include header keys in lowercase
            return _responseHeaders.TryGetValue(key.ToLower(), out var value) ? value : string.Empty;
        }

        public async Task<WebRequestResponse> Send()
        {
            var request = new HttpRequestMessage(_method, _url);
            
            foreach (var headerPair in _headers)
                if (headerPair.Key != "Content-Type")
                    request.Headers.Add(headerPair.Key, headerPair.Value);
            
            request.Content = new ByteArrayContent(_data);
            foreach (var headerPair in _headers)
                if (headerPair.Key == "Content-Type")
                    request.Content.Headers.Add(headerPair.Key, headerPair.Value);
            
            var response = await _client.SendAsync(request);
            var responseData = await response.Content.ReadAsByteArrayAsync();
            
            _responseHeaders.Clear();
            foreach (var responseHeaderPair in response.Headers)
                _responseHeaders.Add(responseHeaderPair.Key.ToLower(), responseHeaderPair.Value.ToList()[0]);

            var result = response.IsSuccessStatusCode ? WebRequestResult.Success : WebRequestResult.Failed;
            Data = responseData;
            Text = Encoding.UTF8.GetString(responseData);
            
            return new WebRequestResponse
            {
                Data = responseData,
                ResponseCode = (int)response.StatusCode,
                Result = result
            };
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}