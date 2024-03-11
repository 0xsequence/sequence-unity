using System;
using System.Runtime.InteropServices;
using AOT;

namespace Sequence.Authentication
{
    public class IosBrowser : IBrowser
    {
        private OpenIdAuthenticator _authenticator;
        private string _urlScheme;
        private static IosBrowser _instance;
        
        private IosBrowser(OpenIdAuthenticator authenticator, string urlScheme)
        {
            _authenticator = authenticator;
            _urlScheme = urlScheme;
        }
        
        public static IosBrowser Setup(OpenIdAuthenticator authenticator, string urlScheme)
        {
            if (authenticator == null)
            {
                throw new ArgumentNullException(nameof(authenticator));
            }
            if (string.IsNullOrWhiteSpace(urlScheme))
            {
                throw new ArgumentNullException(nameof(urlScheme));
            }

            if (_instance == null)
            {
                _instance = new IosBrowser(authenticator, urlScheme);
                return _instance;
            }
            _instance._authenticator = authenticator;
            _instance._urlScheme = urlScheme;
            return _instance;
        }
        
        public void Authenticate(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            
            IntPtr sessionPointer = Auth_ASWebAuthenticationSession_InitWithURL(url, _urlScheme,
                OnAuthenticationSessionCompleted);
            Auth_ASWebAuthenticationSession_SetPrefersEphemeralWebBrowserSession(sessionPointer, 0);
            if (Auth_ASWebAuthenticationSession_Start(sessionPointer) == 0)
            {
                throw new Exception("Failed to start Authentication Session");
            };
        }
        
        [DllImport("__Internal")]
        private static extern IntPtr Auth_ASWebAuthenticationSession_InitWithURL(string url, string callbackUrlScheme, AuthenticationSessionCompletedCallback callback);
        
        [DllImport("__Internal")]
        private static extern int Auth_ASWebAuthenticationSession_Start(IntPtr session);
        
        [DllImport("__Internal")]
        private static extern void Auth_ASWebAuthenticationSession_Cancel(IntPtr session);

        [DllImport("__Internal")]
        private static extern int Auth_ASWebAuthenticationSession_GetPrefersEphemeralWebBrowserSession(IntPtr session);

        [DllImport("__Internal")]
        private static extern void Auth_ASWebAuthenticationSession_SetPrefersEphemeralWebBrowserSession(
            IntPtr session, int enable);
        
        private delegate void AuthenticationSessionCompletedCallback(IntPtr session, string callbackUrl, 
            int errorCode, string errorMessage);
        
        private void mOnAuthenticationSessionCompleted(IntPtr session, string callbackUrl, 
            int errorCode, string errorMessage)
        {
            if (errorMessage != null)
            {
                throw new Exception(errorMessage);
            }
            else if (errorCode != 0)
            {
                throw new Exception($"Error code: {errorCode}");
            }
            else
            {
                _authenticator.HandleDeepLink(callbackUrl);
            }
        }
        
        [MonoPInvokeCallback(typeof(AuthenticationSessionCompletedCallback))]
        private static void OnAuthenticationSessionCompleted(IntPtr session, string callbackUrl, 
            int errorCode, string errorMessage)
        {
            _instance.mOnAuthenticationSessionCompleted(session, callbackUrl, errorCode, errorMessage);
        }
    }
}