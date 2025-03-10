#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

namespace Sequence.Utils
{
    public static class SequencePrefs
    {
        public static bool HasKey(string key)
        {
#if UNITY_2017_1_OR_NEWER
            return PlayerPrefs.HasKey(key);
#else
            return false;
#endif
        }

        public static string GetString(string key)
        {
#if UNITY_2017_1_OR_NEWER
            return PlayerPrefs.GetString(key);
#else
            return false;
#endif
        }
    }
}