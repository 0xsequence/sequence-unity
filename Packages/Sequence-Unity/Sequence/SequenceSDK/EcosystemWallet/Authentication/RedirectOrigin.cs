using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
            return $"{UrlSchemeFactory.CreateFromAppIdentifier()}://";
        }
#else
        public static string GetOriginString()
        {
            return $"http://localhost:{GetAvailablePort()}";
        }
        
        private static int GetAvailablePort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 4444);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
#endif
    }
}