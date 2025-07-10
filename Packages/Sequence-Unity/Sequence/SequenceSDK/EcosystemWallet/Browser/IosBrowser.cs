using System.Runtime.InteropServices;

namespace Sequence.EcosystemWallet.Browser
{
    internal class IosBrowser : IBrowser
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _ShowWebView(string url, float x, float y, float width, float height);

        [DllImport("__Internal")]
        private static extern void _HideWebView();

        [DllImport("__Internal")]
        private static extern void _RemoveWebView();
#else
        private static void _ShowWebView(string url, float x, float y, float width, float height) { }
        private static void _HideWebView() { }
        private static void _RemoveWebView() { }
#endif

        public void Show(string url)
        {
            _ShowWebView(url, 0, 0, 375, 667);
        }

        public void Hide()
        {
            _HideWebView();
        }

        public void Remove()
        {
            _RemoveWebView();
        }
    }
}