using NUnit.Framework;
using Sequence.Extensions;

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
