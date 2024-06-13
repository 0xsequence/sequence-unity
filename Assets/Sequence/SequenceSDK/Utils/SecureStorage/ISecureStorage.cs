namespace Sequence.Utils.SecureStorage
{
    public interface ISecureStorage
    {
        public void StoreString(string key, string value);
        public string RetrieveString(string key);
    }
}