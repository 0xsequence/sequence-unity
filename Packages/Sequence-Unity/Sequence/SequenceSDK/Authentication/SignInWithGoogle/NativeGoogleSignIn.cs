using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.Authentication
{
    public class NativeGoogleSignIn
    {
        private AndroidJavaClass _pluginClass;
        private NativeGoogleSignInReceiver _receiver;

        private string _clientId;
        
        public NativeGoogleSignIn(string clientId)
        {
            _clientId = clientId;
        }

        public async Task<string> SignIn()
        {
            _pluginClass = new AndroidJavaClass("xyz.sequence.GoogleSignInPlugin");
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            
            var done = false;
            var idToken = string.Empty;
            
            _receiver = NativeGoogleSignInReceiver.Create();
            
            Assert.IsNotNull(_receiver, "NativeGoogleSignInReceiver not initialized");
            
            _receiver.OnIdTokenReceived.AddListener(receivedIdToken =>
            {
                idToken = receivedIdToken;
                done = true;
            });
            
            _receiver.OnSignInFailed.AddListener(error =>
            {
                done = true;
                Debug.LogError($"Error during native google sign-in: {error}");
            });
            
            _pluginClass.CallStatic("signIn", activity, _clientId);

            while (!done)
                await Task.Delay(200);
            
            GameObject.Destroy(_receiver.gameObject);
            return idToken;
        }
    }
}