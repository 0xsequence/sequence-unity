using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class NativeReceiver : MonoBehaviour
    {
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
            SequenceLog.Info($"Received response from native plugin: {response}");
            
            _response = response;
        }

        public async Task<string> WaitForResponse(string url)
        {
            SequenceLog.Info($"Opening URL {url}");
            
            OpenWalletApp(url);

            while (string.IsNullOrEmpty(_response))
                await Task.Yield();
            
            return _response;
        }

#if !UNITY_EDITOR && (UNITY_WEBGL || UNITY_IOS)
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void OpenWalletApp(string url);
#elif !UNITY_EDITOR && UNITY_ANDROID
        private static void OpenWalletApp(string url)
        {
            if (!ChromeTabs.IsSupported())
            {
                SequenceLog.Info("Chrome tabs is not supported.");
                return;
            }
            
            ChromeTabs.Open(url);
        }
#else
        private static void OpenWalletApp(string url)
        {
            Application.OpenURL(url);
        }
#endif
    }
}