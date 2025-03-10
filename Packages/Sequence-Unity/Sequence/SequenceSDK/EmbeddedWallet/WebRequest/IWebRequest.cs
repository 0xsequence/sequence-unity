using System;
using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet
{
    public enum WebRequestResult
    {
        Success,
        Failed
    }
    
    public struct WebRequestResponse
    {
        public WebRequestResult Result;
        public int ResponseCode;
        public byte[] Data;
    }
    
    public interface IWebRequest : IDisposable
    {
        string Method { get; }
        string Error { get; }
        string Text { get; }
        byte[] Data { get; }
        void SetRequestData(byte[] data, string contentType = "application/json");
        void SetRequestHeader(string key, string value);
        string GetRequestHeader(string key);
        string GetResponseHeader(string key);
        Task<WebRequestResponse> Send();
    }
}