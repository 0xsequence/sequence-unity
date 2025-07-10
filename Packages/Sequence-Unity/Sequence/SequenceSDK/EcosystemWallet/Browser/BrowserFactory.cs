namespace Sequence.EcosystemWallet.Browser
{
    public static class BrowserFactory
    {
        public static IBrowser CreateBrowser()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new IosBrowser();
#else
            return new WebBrowser();
#endif
        }
    }
}