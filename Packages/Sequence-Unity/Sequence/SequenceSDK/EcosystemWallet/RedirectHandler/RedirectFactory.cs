namespace Sequence.EcosystemWallet.Browser
{
    internal static class RedirectFactory
    {
        public static RedirectHandler CreateHandler()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new IosRedirectHandler();
#elif UNITY_WEBGL && !UNITY_EDITOR
            return new BrowserRedirectHandler();
#else
            return new LocalhostRedirectHandler();
#endif
        }
    }
}