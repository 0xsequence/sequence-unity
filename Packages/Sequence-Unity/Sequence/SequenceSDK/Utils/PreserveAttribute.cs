using System;

namespace Sequence.Utils
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class PreserveAttribute : Attribute
    {
#if UNITY_2017_1_OR_NEWER
        public PreserveAttribute()
        {
            new UnityEngine.Scripting.PreserveAttribute();
        }
#endif
    }
}