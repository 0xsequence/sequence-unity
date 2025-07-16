using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class IosRedirectHandler : RedirectHandler
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _OpenURLInWKWebView(IntPtr urlString);
#else
        private static void _OpenURLInWKWebView(IntPtr urlString) { }
#endif

        public override async Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload)
        {
            Application.OpenURL(ConstructUrl(url, action, payload));
            
            var utf8Bytes = System.Text.Encoding.UTF8.GetBytes(ConstructUrl(url, action, payload) + '\0');
            var urlPtr = Marshal.AllocHGlobal(utf8Bytes.Length);
            Marshal.Copy(utf8Bytes, 0, urlPtr, utf8Bytes.Length);

            _OpenURLInWKWebView(urlPtr);

            await Task.Yield();
            
            return (false, default);
        }
    }
}