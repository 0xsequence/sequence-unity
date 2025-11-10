using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Sequence.Config;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    public static class RedirectOrigin
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern System.IntPtr GetPageOrigin();
            
        public static string GetOriginString()
        {
            return Marshal.PtrToStringAnsi(GetPageOrigin());
        }
#elif !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX)
        public static string GetOriginString()
        {
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.None);
            var urlScheme = string.IsNullOrEmpty(config.UrlScheme)
                ? UrlSchemeFactory.CreateFromAppIdentifier()
                : config.UrlScheme;
            
            return $"{urlScheme}://";
        }
#else
        public static string GetOriginString()
        {
            return $"http://localhost:4444";
        }
#endif
    }
}