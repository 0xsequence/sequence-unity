using System;

namespace Sequence.WaaS.Tests
{
    public static class CustomAssert
    {
        public static Exception TestFailedException(string reason)
        {
            return new Exception(reason);
        }
        public static void NotNull(object obj, string name, params object[] args)
        {
            if (obj == null)
            {
                throw TestFailedException($"{obj} was null | Source: {name} with args {args}");
            }
        }
        
        public static void IsTrue(bool condition, string name, params object[] args)
        {
            if (!condition)
            {
                throw TestFailedException($"Condition was not true | Source: {name} with args {args}");
            }
        }
        
        public static void IsEqual(object expected, object actual, string name, params object[] args)
        {
            if (!expected.Equals(actual))
            {
                throw TestFailedException($"Expected {expected} is not equal to Actual {actual} | Source: {name} with args {args}");
            }
        }
    }
}