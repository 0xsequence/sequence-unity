namespace Sequence.EcosystemWallet.Browser
{
    internal static class RedirectFactory
    {
        public static RedirectHandler CreateHandler()
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            return new IosRedirectHandler();
#elif !UNITY_EDITOR && UNITY_WEBGL
            return new BrowserRedirectHandler();
#else
            return new LocalhostRedirectHandler();
#endif
        }
    }
}