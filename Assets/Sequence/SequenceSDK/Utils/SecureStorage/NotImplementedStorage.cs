using System;

namespace Sequence.Utils
{
    public class NotImplementedStorage : ISecureStorage
    {
        public void StoreString(string key, string value)
        {
            throw new NotImplementedException("Secure storage is not currently implemented for this platform.");
        }

        public string RetrieveString(string key)
        {
            throw new NotImplementedException("Secure storage is not currently implemented for this platform.");
        }
    }
}