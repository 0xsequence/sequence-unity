using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sequence.Utils
{
    public static class TupleExtensions
    {
        public static bool IsTuple(this object value)
        {
            if (value == null)
            {
                return false;
            }
            Type type = value.GetType();
            return type.IsGenericType &&
                    (type.FullName.StartsWith("System.Tuple") || type.FullName.StartsWith("System.ValueTuple"));
        }
        
        public static List<object> ToObjectList(this object value)
        {
            if (!value.IsTuple())
            {
                throw new ArgumentException("Must provide a Tuple or ValueTuple as arguments, given: " + value.GetType());
            }
            
            ITuple tuple = (ITuple)value;
            int length = tuple.Length;
            List<object> list = new List<object>();
            for (int i = 0; i < length; i++)
            {
                list.Add(tuple[i]);
            }
            
            return list;
        }
    }
}