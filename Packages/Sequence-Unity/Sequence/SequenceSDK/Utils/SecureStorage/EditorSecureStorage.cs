namespace Sequence.Utils.SecureStorage
{
    public class EditorSecureStorage : ISecureStorage
    {
        public void StoreString(string key, string value)
        {
            SequencePrefs.SetString(key, value);
            SequencePrefs.Save();
        }

        public string RetrieveString(string key)
        {
            return SequencePrefs.GetString(key);
        }

        public EditorSecureStorage()
        {
        }
    }
}