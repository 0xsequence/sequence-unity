
namespace Sequence.Utils.SecureStorage
{
    public class WebSecureStorage : ISecureStorage
    {
        public WebSecureStorage()
        {
            
#if !UNITY_WEBGL || UNITY_EDITOR
            throw new System.NotSupportedException("WebSecureStorage is only supported on Web platforms.");
#endif
        }
        
        // PlayerPrefs uses IndexedDB on web platforms https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
        public void StoreString(string key, string value)
        {
            SequencePrefs.SetString(key, value);
            SequencePrefs.Save();
        }

        public string RetrieveString(string key)
        {
            return SequencePrefs.GetString(key);
        }
    }
}