using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class NativeReceiver : MonoBehaviour
    {
        private string _redirectUrl;
        private string _response;

        private void Awake()
        {
            Application.deepLinkActivated += HandleResponse;
        }

        private void OnDestroy()
        {
            Application.deepLinkActivated -= HandleResponse;
        }

        public void HandleResponse(string response)
        {
            Debug.Log($"Received response from native plugin: {response}");
            
            _response = response;
        }

        public async Task<string> WaitForResponse(string url, string redirectUrl)
        {
            Debug.Log($"Opening URL {url}");
            
            _redirectUrl = redirectUrl.Replace("://", "");
            OpenWalletApp(url);

            while (string.IsNullOrEmpty(_response))
                await Task.Yield();
            
            return _response;
        }

#if !UNITY_EDITOR && (UNITY_WEBGL)
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void OpenWalletApp(string url);
#elif !UNITY_EDITOR && UNITY_IOS
        private void OpenWalletApp(string url)
        {
            ASWebAuth_Start(url, _redirectUrl);
        }
        
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void ASWebAuth_Start(string url, string callbackScheme);
        
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void ASWebAuth_Cancel();
#else
        private static void OpenWalletApp(string url)
        {
            Application.OpenURL(url);
        }
#endif
    }
}