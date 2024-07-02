using NUnit.Framework;
using Sequence.Utils;

namespace Sequence.Utils.Tests
{
    public class BoolExtensionsTests
    {
        [TestCase(true, new byte[] { 1 })]
        [TestCase(false, new byte[] { 0 })]
        public void TestToByteArray(bool value, byte[] expected)
        {
            byte[] byteArray = value.ToByteArray();

            Assert.AreEqual(expected, byteArray);
        }
    }
}
