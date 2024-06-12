using System;

namespace Sequence.Utils
{
    public static class SecureStorageFactory
    {
        public static ISecureStorage CreateSecureStorage()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new iOSKeychainStorage();
#else
            throw new NotImplementedException("Secure storage is not currently implemented for this platform.");
#endif
        }

        public static bool IsSupportedPlatform()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
}