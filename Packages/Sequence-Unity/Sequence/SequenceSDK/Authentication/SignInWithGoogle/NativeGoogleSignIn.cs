using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.Authentication
{
    public class NativeGoogleSignIn
    {
        private AndroidJavaClass _pluginClass;
        private NativeGoogleSignInReceiver _receiver;
        
        public NativeGoogleSignIn(string clientId)
        {
            _pluginClass = new AndroidJavaClass("xyz.sequence.GoogleSignInPlugin");
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _pluginClass.CallStatic("initialize", activity, clientId);
        }

        public async Task<string> SignIn()
        {
            var done = false;
            var idToken = string.Empty;
            
            _receiver = NativeGoogleSignInReceiver.Create();
            
            Assert.IsNotNull(_receiver, "NativeGoogleSignInReceiver not initialized");
            Debug.Log($"SignIn");
            _receiver.OnIdTokenReceived.AddListener(receivedIdToken =>
            {
                idToken = receivedIdToken;
                done = true;
                Debug.Log($"receivedIdToken: {receivedIdToken}");
            });
            
            _receiver.OnSignInFailed.AddListener(error =>
            {
                done = true;
                Debug.LogError($"Error during native google sign-in: {error}");
            });
            
            _pluginClass.CallStatic("signIn");

            while (!done)
                await Task.Delay(200);
            Debug.Log($"SignIn Done");
            GameObject.Destroy(_receiver);
            return idToken;
        }
    }
}