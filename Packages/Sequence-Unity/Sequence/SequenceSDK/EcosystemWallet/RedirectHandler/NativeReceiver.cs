using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class NativeReceiver : MonoBehaviour
    {
        private string _response;

        public void HandleResponse(string response)
        {
            _response = response;
        }

        public async Task<string> WaitForResponse(string url)
        {
            OpenWalletApp(url);

            while (string.IsNullOrEmpty(_response))
                await Task.Yield();
            
            return _response;
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenWalletApp(string url);
#else
        private static void OpenWalletApp(string url) { }
#endif
    }
}