using UnityEngine.Assertions;

namespace SequenceExamples.Scripts.Tests
{
    public static class TestUtils
    {
        public static void AssertStartsWith(this string value, string startsWith)
        {
            int length = startsWith.Length;
            Assert.AreEqual(startsWith, value.Substring(0, length));
        }
    }
}