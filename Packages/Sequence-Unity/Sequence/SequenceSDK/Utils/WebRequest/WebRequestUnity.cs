#if UNITY_2017_1_OR_NEWER
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Sequence.Utils
{
    public class WebRequestUnity : IWebRequest
    {
        public string Method { get; private set; }
        public string Error { get; private set; }
        public string Text { get; private set; }
        public byte[] Data { get; private set; }

        private readonly UnityWebRequest _request;
        
        public WebRequestUnity(string url, string method)
        {
            Method = method;
            _request = UnityWebRequest.Get(url);
            _request.method = method;
        }

        public void SetRequestData(byte[] data, string contentType)
        {
            _request.uploadHandler = new UploadHandlerRaw(data);
            _request.uploadHandler.contentType = contentType;
        }

        public void SetRequestHeader(string key, string value)
        {
            _request.SetRequestHeader(key, value);
        }

        public void SetTimeout(int timeout)
        {
            _request.timeout = timeout;
        }

        public string GetRequestHeader(string key)
        {
            return _request.GetRequestHeader(key);
        }

        public string GetResponseHeader(string key)
        {
            return _request.GetResponseHeader(key);
        }

        public async Task<WebRequestResponse> Send()
        {
            await _request.SendWebRequest();
            Data = _request.downloadHandler.data;
            Text = _request.downloadHandler.text;
            Error = _request.error;
            
            return new WebRequestResponse
            {
                Data = Data,
                ResponseCode = (int)_request.responseCode,
                Result = _request.result == UnityWebRequest.Result.Success ? 
                    WebRequestResult.Success : WebRequestResult.Failed
            };
        }
        
        public void Dispose()
        {
            _request.Dispose();
        }
    }
}

#endif
