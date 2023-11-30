using System;

namespace Sequence.WaaS.Tests
{
    public static class CustomAssert
    {
        public static Exception TestFailedException;
        public static void NotNull(object obj, string name, params object[] args)
        {
            if (obj == null)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(name, args));
                throw TestFailedException;
            }
        }
        
        public static void IsTrue(bool condition, string name, params object[] args)
        {
            if (!condition)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(name, args));
                throw TestFailedException;
            }
        }
        
        public static void IsEqual(object expected, object actual, string name, params object[] args)
        {
            if (!expected.Equals(actual))
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(name, args));
                throw TestFailedException;
            }
        }
    }
}