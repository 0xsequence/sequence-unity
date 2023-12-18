using System;
using NUnit.Framework;
using Sequence.Extensions;

public class UInt16ExtensionsTests
{
    [TestCase((UInt16)0x0000, new byte[] { 0x00, 0x00 })]
    [TestCase((UInt16)0x1234, new byte[] { 0x12, 0x34 })]
    [TestCase((UInt16)0xABCD, new byte[] { 0xAB, 0xCD })]
    [TestCase((UInt16)0xFFFF, new byte[] { 0xFF, 0xFF })]
    public void TestToByteArray(UInt16 value, byte[] expected)
    {
        byte[] byteArray = value.ToByteArray();

        Assert.AreEqual(expected, byteArray);
    }
}
