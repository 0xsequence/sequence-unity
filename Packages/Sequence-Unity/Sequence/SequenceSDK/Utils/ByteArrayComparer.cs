using System;
using System.Collections.Generic;

namespace Sequence.Utils
{
    public class ByteArrayComparer : IComparer<byte[]>
    {
        public int Compare(byte[]? x, byte[]? y)
        {
            if (x == null || y == null)
                throw new ArgumentException("Cannot compare null byte arrays");
            
            for (var i = 0; i < Math.Min(x.Length, y.Length); i++)
            {
                var cmp = x[i].CompareTo(y[i]);
                if (cmp != 0)
                    return cmp;
            }
            return x.Length.CompareTo(y.Length);
        }
    }
}