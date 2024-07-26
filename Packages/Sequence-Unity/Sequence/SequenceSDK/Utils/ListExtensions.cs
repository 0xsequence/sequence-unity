using System.Collections.Generic;

namespace Sequence.Utils
{
    public static class ListExtensions
    {
        public static List<T> RemoveItemsInList<T>(this List<T> original, List<T> toExclude)
        {
            List<T> newList = new List<T>();
            int length = original.Count;

            for (int i = 0; i < length; i++)
            {
                T item = original[i];
                if (!toExclude.Contains(item))
                {
                    newList.Add(item);
                }
            }

            return newList;
        }
    }
}