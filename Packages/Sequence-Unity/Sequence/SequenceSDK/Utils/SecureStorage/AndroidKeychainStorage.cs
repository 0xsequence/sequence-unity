using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Sequence.Utils.SecureStorage
{
    public class AndroidKeychainStorage : ISecureStorage
    {
        private bool _isInitialized = false;

        public AndroidKeychainStorage()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            Debug.LogError("AndroidKeychainStorage is only supported on Android platform.");
            throw new System.NotSupportedException("AndroidKeychainStorage is only supported on Android platform.");
#else
            Debug.Log("AndroidKeychainStorage constructor called.");
            InitializeAndroidKeyBridge();
#endif
        }

#if UNITY_ANDROID
        private void InitializeAndroidKeyBridge()
        {
            if (!_isInitialized)
            {
                Debug.Log("Initializing AndroidKeyBridge...");
                using (AndroidJavaClass bridgeClass = new AndroidJavaClass("xyz.sequence.AndroidKeyBridge"))
                {
                    if (bridgeClass == null)
                    {
                        Debug.LogError("Failed to create AndroidJavaClass for AndroidKeyBridge.");
                        return;
                    }

                    AndroidJavaObject unityContext = GetUnityActivity();
                    if (unityContext == null)
                    {
                        Debug.LogError("Failed to retrieve Unity activity context.");
                        return;
                    }

                    try
                    {
                        bridgeClass.Call("init", unityContext);
                        Debug.Log("AndroidKeyBridge initialized successfully.");
                        _isInitialized = true;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Exception while initializing AndroidKeyBridge: {e.Message}");
                    }
                }
            }
            else
            {
                Debug.Log("AndroidKeyBridge is already initialized.");
            }
        }

        private AndroidJavaObject GetUnityActivity()
        {
            Debug.Log("Attempting to get Unity activity...");
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Failed to get UnityPlayer class.");
                    return null;
                }

                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity == null)
                {
                    Debug.LogError("Failed to get current Unity activity.");
                }
                else
                {
                    Debug.Log("Unity activity obtained successfully.");
                }
                return activity;
            }
        }
#endif

        public void StoreString(string key, string value)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log($"Storing string in AndroidKeyBridge: key={key}, value={value}");
            try
            {
                AndroidJavaClass androidBridge = new AndroidJavaClass("xyz.sequence.AndroidKeyBridge");
                if (androidBridge == null)
                {
                    Debug.LogError("Failed to create AndroidJavaClass for AndroidKeyBridge in StoreString.");
                    return;
                }
                androidBridge.CallStatic("SaveKeychainValue", key, value);
                Debug.Log("String stored successfully in AndroidKeyBridge.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception in StoreString: {e.Message}");
            }
#endif
        }

        public string RetrieveString(string key)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log($"Retrieving string from AndroidKeyBridge: key={key}");
            try
            {
                AndroidJavaClass androidBridge = new AndroidJavaClass("xyz.sequence.AndroidKeyBridge");
                if (androidBridge == null)
                {
                    Debug.LogError("Failed to create AndroidJavaClass for AndroidKeyBridge in RetrieveString.");
                    return null;
                }
                string result = androidBridge.CallStatic<string>("GetKeychainValue", key);
                Debug.Log($"String retrieved successfully from AndroidKeyBridge: key={key}, value={result}");
                return result;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception in RetrieveString: {e.Message}");
                return null;
            }
#else
            Debug.LogWarning("RetrieveString called on non-Android platform or in editor.");
            return null;
#endif
        }
    }
}