using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Sequence.Utils
{
    public class iOSKeychainStorage : ISecureStorage
    {
        public iOSKeychainStorage()
        {
#if !UNITY_IOS || UNITY_EDITOR
            throw new System.NotSupportedException("iOSKeychainStorage is only supported on iOS platform.");
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SaveKeychainValue(string key, string value);

        [DllImport("__Internal")]
        private static extern IntPtr GetKeychainValue(string key);
#else
        private static void SaveKeychainValue(string key, string value) { }
        private static IntPtr GetKeychainValue(string key) { return IntPtr.Zero; }
#endif
        
        public void StoreString(string key, string value)
        {
            SaveKeychainValue(DecorateKey(key), value);
        }

        public string RetrieveString(string key)
        {
            IntPtr valuePtr = GetKeychainValue(DecorateKey(key));
            if (valuePtr != IntPtr.Zero)
            {
                string value = Marshal.PtrToStringUTF8(valuePtr);
                Marshal.FreeHGlobal(valuePtr); // Free the duplicated memory
                return value;
            }
            return null;
        }

        private string DecorateKey(string key)
        {
            return Application.companyName + "-" + Application.productName + "-" + key; // This should help ensure the key is unique for each app
        }
    }
}