using System.Runtime.InteropServices;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Authentication
{
    public static class RedirectOrigin
    {
        public const string DefaultOrigin = "http://localhost:8080/";
        
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
            return $"{UrlSchemeFactory.CreateFromAppIdentifier()}://";
        }
#else
        public static string GetOriginString()
        {
            return DefaultOrigin;
        }
#endif
    }
}