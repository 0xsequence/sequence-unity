using System.Collections.Generic;

namespace Sequence.Utils
{
    public static class DictionaryExtensions
    {
        public static bool HaveMatchingKeys<TKey, TValue1, TValue2>(
            Dictionary<TKey, TValue1> dict1,
            Dictionary<TKey, TValue2> dict2)
        {
            if (dict1.Count != dict2.Count)
            {
                return false;
            }

            foreach (TKey key in dict1.Keys)
            {
                if (!dict2.ContainsKey(key))
                {
                    return false;
                }
            }

            return true;
        }
        
        public static TKey[] GetKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            TKey[] keys = new TKey[dict.Count];
            int index = 0;

            foreach (TKey key in dict.Keys)
            {
                keys[index++] = key;
            }

            return keys;
        }
    }
}