using System.Runtime.InteropServices;
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
#elif !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        public static string GetOriginString()
        {
            return $"{CreateUrlScheme()}://";
        }
#else
        public static string GetOriginString()
        {
            return DefaultOrigin;
        }
#endif

        public static string CreateUrlScheme()
        {
            return Application.identifier.Replace(".", "").ToLower();
        }
    }
}