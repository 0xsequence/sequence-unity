using UnityEngine;

namespace Sequence.Utils.SecureStorage
{
    public class EditorSecureStorage : ISecureStorage
    {
        public void StoreString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public string RetrieveString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public EditorSecureStorage()
        {
        }
    }
}