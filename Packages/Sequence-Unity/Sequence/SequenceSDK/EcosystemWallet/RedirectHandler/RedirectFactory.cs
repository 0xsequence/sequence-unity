namespace Sequence.EcosystemWallet.Browser
{
    public static class RedirectFactory
    {
        public static IRedirectHandler CreateHandler()
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