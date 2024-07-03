using System;

namespace Sequence.Utils.SecureStorage
{
    public static class SecureStorageFactory
    {
        public static ISecureStorage CreateSecureStorage()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new iOSKeychainStorage();
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return new MacOSKeychainStorage();
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