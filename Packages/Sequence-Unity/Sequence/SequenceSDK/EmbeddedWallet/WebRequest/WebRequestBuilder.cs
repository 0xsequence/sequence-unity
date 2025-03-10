namespace Sequence.EmbeddedWallet
{
    public static class WebRequestBuilder
    {
        public static IWebRequest Get(string url)
        {
            return Create(url, "GET");
        }

        public static IWebRequest Post(string url)
        {
            return Create(url, "POST");
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