using System.Text;

namespace Sequence.Utils
{
    public static class WebRequestBuilder
    {
        public static IWebRequest Get(string url)
        {
            return Create(url, "GET");
        }

        public static IWebRequest Post(string url, string payload = "")
        {
            var request = Create(url, "POST");
            if (!string.IsNullOrEmpty(payload))
                request.SetRequestData(Encoding.UTF8.GetBytes(payload));
            
            return request;
        }

        private static IWebRequest Create(string url, string method)
        {
#if UNITY_2017_1_OR_NEWER
            return new WebRequestUnity(url, method);
#else
            return new WebRequestDotnet(url, method);
#endif
        }
    }
}