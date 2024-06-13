namespace Sequence.Utils
{
    public interface ISecureStorage
    {
        public void StoreString(string key, string value);
        public string RetrieveString(string key);
    }
}