using UnityEngine;
using UnityEngine.Events;

namespace Sequence.Authentication
{
    public class NativeGoogleSignInReceiver : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<string> OnIdTokenReceived = new();
        [HideInInspector] public UnityEvent<string> OnSignInFailed = new();

        public static NativeGoogleSignInReceiver Create()
        {
            return new GameObject(nameof(NativeGoogleSignInReceiver)).AddComponent<NativeGoogleSignInReceiver>();
        }
        
        public void HandleIdToken(string idToken)
        {
            OnIdTokenReceived.Invoke(idToken);
        }

        public void HandleError(string error)
        {
            OnSignInFailed.Invoke(error);
        }
    }
}