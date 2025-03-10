using System;
using System.Runtime.InteropServices;

namespace Sequence.Utils.SecureStorage
{
    public class MacOSKeychainStorage : ISecureStorage
    {
        public MacOSKeychainStorage()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
            throw new System.NotSupportedException("MacOSKeychainStorage is only supported on macOS platform.");
#endif
        }

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        [DllImport("KeychainAccess")]
        private static extern void SaveKeychainValue(string key, string value);

        [DllImport("KeychainAccess")]
        private static extern IntPtr GetKeychainValue(string key);
#else
        private static void SaveKeychainValue(string key, string value) { }
        private static IntPtr GetKeychainValue(string key) { return IntPtr.Zero; }
#endif
        
        public void StoreString(string key, string value)
        {
            SaveKeychainValue(key, value);
        }

        public string RetrieveString(string key)
        {
            IntPtr valuePtr = GetKeychainValue(key);
            if (valuePtr != IntPtr.Zero)
            {
                string value = Marshal.PtrToStringUTF8(valuePtr);
                Marshal.FreeHGlobal(valuePtr); // Free the duplicated memory
                return value;
            }
            return null;
        }
    }
}