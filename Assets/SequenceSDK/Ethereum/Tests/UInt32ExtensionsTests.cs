using System;
using NUnit.Framework;
using Sequence.Extensions;

public class UInt32ExtensionsTests
{
    [TestCase((UInt32)0x00000000, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
    [TestCase((UInt32)0x12345678, new byte[] { 0x12, 0x34, 0x56, 0x78 })]
    [TestCase((UInt32)0xAABBCCDD, new byte[] { 0xAA, 0xBB, 0xCC, 0xDD })]
    [TestCase((UInt32)0xFFFFFFFF, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
    public void TestToByteArray(UInt32 value, byte[] expected)
    {
        byte[] byteArray = value.ToByteArray();

        Assert.AreEqual(expected, byteArray);
    }
}
