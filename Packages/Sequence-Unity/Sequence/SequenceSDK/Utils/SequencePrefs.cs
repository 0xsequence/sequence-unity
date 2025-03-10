#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

namespace Sequence.Utils
{
    public static class SequencePrefs
    {
#if UNITY_2017_1_OR_NEWER
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
#else
        public static bool HasKey(string key)
        {
            return false;
        }

        public static string GetString(string key)
        {
            return "";
        }

        public static void SetInt(string key, int value)
        {
            
        }

        public static void SetString(string key, string value)
        {
            
        }
        
        public static void Save()
        {

        }
#endif
    }
}