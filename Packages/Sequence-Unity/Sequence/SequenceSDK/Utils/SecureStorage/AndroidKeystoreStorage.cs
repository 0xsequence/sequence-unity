using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Sequence.Utils.SecureStorage
{
    public class AndroidKeystoreStorage : ISecureStorage
    {
        private bool _isInitialized = false;
        
        public AndroidKeystoreStorage()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            throw new System.NotSupportedException("AndroidKeystoreStorage is only supported on Android platform.");
#elif UNITY_ANDROID && ENABLE_SEQUENCE_ANDROID_SECURE_STORAGE
            InitializeAndroidKeyBridge();
#else
            throw new System.NotSupportedException("Invalid use. Secure storage is not enabled. Please enable in SequenceConfig and/or set the script define above");
#endif
        }

#if UNITY_ANDROID && ENABLE_SEQUENCE_ANDROID_SECURE_STORAGE
        private void InitializeAndroidKeyBridge()
        {
            if (!_isInitialized)
            {
                using (AndroidJavaClass javaClass = new AndroidJavaClass("xyz.sequence.AndroidKeyBridge"))
                {
                    AndroidJavaObject bridgeObject = javaClass.CallStatic<AndroidJavaObject>("getInstance");
                    {
                        AndroidJavaObject unityContext = GetUnityActivity();
                        bridgeObject.Call("init", unityContext);
                        _isInitialized = true;
                    }
                }
            }
        }
        
        private AndroidJavaObject GetUnityActivity()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
#endif
        
        public void StoreString(string key, string value)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR && ENABLE_SEQUENCE_ANDROID_SECURE_STORAGE
                AndroidJavaClass androidBridge = new AndroidJavaClass("xyz.sequence.AndroidKeyBridge");
                androidBridge.CallStatic("SaveKeychainValue", key, value);
            #endif
        }

        public string RetrieveString(string key)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR && ENABLE_SEQUENCE_ANDROID_SECURE_STORAGE
                AndroidJavaClass androidBridge = new AndroidJavaClass("xyz.sequence.AndroidKeyBridge");
                return androidBridge.CallStatic<string>("GetKeychainValue", key);
            #else
                return null;
            #endif
        }
    }
}