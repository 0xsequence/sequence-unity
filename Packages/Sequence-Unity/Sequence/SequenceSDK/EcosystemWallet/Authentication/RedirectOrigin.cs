using System.Runtime.InteropServices;

namespace Sequence.EcosystemWallet.Authentication
{
    internal static class RedirectOrigin
    {
        public const string DefaultOrigin = "http://localhost:8080/";
        
#if UNITY_WEBGL &&! UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern System.IntPtr GetPageOrigin();
            
        public static string GetOriginString()
        {
            return Marshal.PtrToStringAnsi(GetPageOrigin());
        }
#elif UNITY_IOS && UNITY_EDITOR
        public static string GetOriginString()
        {
            return "sequencedemo://auth/callback";
        }
#else
        public static string GetOriginString()
        {
            return DefaultOrigin;
        }
#endif
    }
}