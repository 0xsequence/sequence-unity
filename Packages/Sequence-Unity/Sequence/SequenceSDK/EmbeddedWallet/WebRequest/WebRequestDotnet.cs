using System.Net.Http;
using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet
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
        
        public WebRequestDotnet(string url, string method)
        {
            _client = new System.Net.Http.HttpClient();
            _url = url;

            _method = method switch
            {
                "POST" => HttpMethod.Post,
                _ => HttpMethod.Get
            };
            
            _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        }
        
        public void SetRequestData(byte[] data, string contentType = "application/json")
        {
            throw new System.NotImplementedException();
        }

        public void SetRequestHeader(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public string GetRequestHeader(string key)
        {
            throw new System.NotImplementedException();
        }

        public string GetResponseHeader(string key)
        {
            throw new System.NotImplementedException();
        }

        public async Task<WebRequestResponse> Send()
        {
            await _client.SendAsync(new HttpRequestMessage(_method, _url));
            return new WebRequestResponse();
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}