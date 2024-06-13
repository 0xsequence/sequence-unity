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
            return new NotImplementedStorage();
#endif
        }

        public static bool IsSupportedPlatform()
        {
            ISecureStorage secureStorage = CreateSecureStorage();
            return secureStorage is not NotImplementedStorage;
        }
    }
}