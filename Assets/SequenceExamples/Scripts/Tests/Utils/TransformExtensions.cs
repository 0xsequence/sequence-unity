using UnityEngine;

namespace SequenceExamples.Scripts.Tests.Utils
{
    public static class TransformExtensions
    {
        public static Transform FindAmongDecendants(this Transform parent, string name)
        {
            if (parent == null)
            {
                return null;
            }

            Transform result = parent.Find(name);
            if (result != null)
            {
                return result;
            }

            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                result = parent.GetChild(i).FindAmongDecendants(name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}