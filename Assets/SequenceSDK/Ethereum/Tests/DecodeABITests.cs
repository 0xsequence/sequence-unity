using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using NUnit.Framework;
using Sequence;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Utils;
using UnityEngine;
using Object = System.Object;

public class DecodeABITests
{
    private static object[] DecodeAbiTests =
    {
        new object[]
        {
            ERC20.Abi, new Dictionary<string, List<(string[], string)>>
            {
                { "name", new List<(string[], string)> { (new string[] { }, "(string)") } },
                { "allowance", new List<(string[], string)> { (new string[] { "address", "address" }, "(uint256)") } },
                { "approve", new List<(string[], string)> { (new string[] { "address", "uint256" }, "(bool)") } },
                { "balanceOf", new List<(string[], string)> { (new string[] { "address" }, "(uint256)") } },
                { "burn", new List<(string[], string)> { (new string[] { "uint256" }, null) } },
                { "burnFrom", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "decimals", new List<(string[], string)> { (new string[] { }, "(uint8)") } },
                {
                    "decreaseAllowance",
                    new List<(string[], string)> { (new string[] { "address", "uint256" }, "(bool)") }
                },
                {
                    "increaseAllowance",
                    new List<(string[], string)> { (new string[] { "address", "uint256" }, "(bool)") }
                },
                { "mint", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "owner", new List<(string[], string)> { (new string[] { }, "(address)") } },
                { "renounceOwnership", new List<(string[], string)> { (new string[] { }, null) } },
                { "symbol", new List<(string[], string)> { (new string[] { }, "(string)") } },
                { "totalSupply", new List<(string[], string)> { (new string[] { }, "(uint256)") } },
                { "transfer", new List<(string[], string)> { (new string[] { "address", "uint256" }, "(bool)") } },
                {
                    "transferFrom",
                    new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, "(bool)") }
                },
                { "transferOwnership", new List<(string[], string)> { (new string[] { "address" }, null) } },
            },
        },
        new object[]
        {
            ERC721.Abi, new Dictionary<string, List<(string[], string)>>
            {
                { "approve", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "balanceOf", new List<(string[], string)> { (new string[] { "address" }, "(uint256)") } },
                { "burn", new List<(string[], string)> { (new string[] { "uint256" }, null) } },
                { "getApproved", new List<(string[], string)> { (new string[] { "uint256" }, "(address)") } },
                {
                    "isApprovedForAll", new List<(string[], string)> { (new string[] { "address", "address" }, "(bool)") }
                },
                { "name", new List<(string[], string)> { (new string[] { }, "(string)") } },
                { "owner", new List<(string[], string)> { (new string[] { }, "(address)") } },
                { "ownerOf", new List<(string[], string)> { (new string[] { "uint256" }, "(address)") } },
                { "renounceOwnership", new List<(string[], string)> { (new string[] { }, null) } },
                { "safeMint", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                {
                    "safeTransferFrom",
                    new List<(string[], string)>
                    {
                        (new string[] { "address", "address", "uint256" }, null),
                        (new string[] { "address", "address", "uint256", "bytes" }, null)
                    }
                },
                { "setApprovalForAll", new List<(string[], string)> { (new string[] { "address", "bool" }, null) } },
                { "supportsInterface", new List<(string[], string)> { (new string[] { "bytes4" }, "(bool)") } },
                { "symbol", new List<(string[], string)> { (new string[] { }, "(string)") } },
                { "tokenURI", new List<(string[], string)> { (new string[] { "uint256" }, "(string)") } },
                {
                    "transferFrom",
                    new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, null) }
                },
                { "transferOwnership", new List<(string[], string)> { (new string[] { "address" }, null) } },
            },
        },
        new object[]
        {
            ERC1155.Abi, new Dictionary<string, List<(string[], string)>>
            {
                { "balanceOf", new List<(string[], string)> { (new string[] { "address", "uint256" }, "(uint256)") } },
                {
                    "balanceOfBatch",
                    new List<(string[], string)> { (new string[] { "address[]", "uint256[]" }, "(uint256[])") }
                },
                { "burn", new List<(string[], string)> { (new string[] { "address", "uint256", "uint256" }, null) } },
                {
                    "burnBatch",
                    new List<(string[], string)> { (new string[] { "address", "uint256[]", "uint256[]" }, null) }
                },
                { "exists", new List<(string[], string)> { (new string[] { "uint256" }, "(bool)") } },
                {
                    "isApprovedForAll", new List<(string[], string)> { (new string[] { "address", "address" }, "(bool)") }
                },
                {
                    "mint",
                    new List<(string[], string)> { (new string[] { "address", "uint256", "uint256", "bytes" }, null) }
                },
                {
                    "mintBatch",
                    new List<(string[], string)>
                        { (new string[] { "address", "uint256[]", "uint256[]", "bytes" }, null) }
                },
                { "setApprovalForAll", new List<(string[], string)> { (new string[] { "address", "bool" }, null) } },
                { "setURI", new List<(string[], string)> { (new string[] { "string" }, null) } },
                { "supportsInterface", new List<(string[], string)> { (new string[] { "bytes4" }, "(bool)") } },
                { "totalSupply", new List<(string[], string)> { (new string[] { "uint256" }, "(uint256)") } },
                { "transferOwnership", new List<(string[], string)> { (new string[] { "address" }, null) } },
                { "uri", new List<(string[], string)> { (new string[] { "uint256" }, "(string)") } },
                { "owner", new List<(string[], string)> { (new string[] { }, "(address)") } },
                { "renounceOwnership", new List<(string[], string)> { (new string[] { }, null) } },
                {
                    "safeTransferFrom",
                    new List<(string[], string)>
                        { (new string[] { "address", "address", "uint256", "uint256", "bytes" }, null) }
                },
                {
                    "safeBatchTransferFrom",
                    new List<(string[], string)>
                        { (new string[] { "address", "address", "uint256[]", "uint256[]", "bytes" }, null) }
                },
            },
        },
    };

    [TestCaseSource(nameof(DecodeAbiTests))]
    public void TestDecodeAbi(string abi, Dictionary<string, List<(string[], string)>> expected)
    {
        try
        {
            Dictionary<string, List<(string[], string)>> result = ABI.DecodeAbi(abi).Abi;

            Assert.NotNull(result);
            var expectedAbi = new FunctionAbi(expected);
            var resultAbi = new FunctionAbi(result);
            Assert.True(expectedAbi.IsEqualTo(resultAbi),
                $"{expectedAbi.GetType()} do not match. Expected: {expectedAbi.ToString()} Received: {resultAbi.ToString()}");
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    public static string longMultiLineString = @"this is a
multi-line string
that is also rather long and pointless
but if I just keep typing maybe it will make for an interesting test case
and maybe,
just maybe,
you as a reader will develop an ever so slight smile on your face
and in that case, my mission will be complete.";

    private static object[] DecodeStringTests =
    {
        new string[] { "banana", "0x0000000000000000000000000000000000000000000000000000000000000006" + "62616e616e61" },
        new string[] { "welcome to Horizon", "0x0000000000000000000000000000000000000000000000000000000000000012" + "77656c636f6d6520746f20486f72697a6f6e" },
        new string[]
        {
            longMultiLineString,
            "0x000000000000000000000000000000000000000000000000000000000000011b" +
            "7468697320697320610d0a6d756c74692d6c696e6520737472696e670d0a74686174" +
            "20697320616c736f20726174686572206c6f6e6720616e6420706f696e746c657373" +
            "0d0a6275742069662049206a757374206b65657020747970696e67206d6179626520" +
            "69742077696c6c206d616b6520666f7220616e20696e746572657374696e67207465" +
            "737420636173650d0a616e64206d617962652c0d0a6a757374206d617962652c0d0a" +
            "796f752061732061207265616465722077696c6c20646576656c6f7020616e206576" +
            "657220736f20736c6967687420736d696c65206f6e20796f757220666163650d0a61" +
            "6e6420696e207468617420636173652c206d79206d697373696f6e2077696c6c2062" +
            "6520636f6d706c6574652e"
        },
    };


    [TestCaseSource(nameof(DecodeStringTests))]
    public void TestDecodeString(string expected, string value)
    {
        try
        {
            string result = ABI.Decode<string>(value, "string");
            Assert.AreEqual(expected, result);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeStringThrowsTests =
    {
        new object[] { BigInteger.One },
        new object[] { Encoding.UTF8.GetBytes("banana") },
        new object[] { 5 },
        new object[] { new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249") },
        new object[] { true },
    };

    [TestCaseSource(nameof(DecodeStringThrowsTests))]
    public void TestDecodeString_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "string");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'string\'. Supported types: {typeof(string)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }

    private static object[] DecodeAddressTests =
    {
        new object[]
        {
            new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249"),
            "0xc683a014955b75F5ECF991d4502427c8fa1Aa249"
        },
        new object[]
        {
            "0xc683a014955B75F5ECF991D4502427C8FA1AA249",
            "0xc683a014955b75F5ECF991d4502427c8fa1Aa249"
        },
        new object[]
        {
            new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249"),
            "0x0000000c683a014955b75F5ECF991d4502427c8fa1Aa249"
        },
        new object[]
        {
            "0xc683a014955B75F5ECF991D4502427C8FA1AA249",
            "0x00000000000c683a014955b75F5ECF991d4502427c8fa1Aa249"
        },
    };

    [TestCaseSource(nameof(DecodeAddressTests))]
    public void TestDecodeAddress<T>(T expected, string value)
    {
        try
        {
            T result = ABI.Decode<T>(value, "address");
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeAddressThrowsTests =
    {
        new object[] { BigInteger.One },
        new object[] { Encoding.UTF8.GetBytes("banana") },
        new object[] { 5 },
        new object[] { true },
    };

    [TestCaseSource(nameof(DecodeAddressThrowsTests))]
    public void TestDecodeAddress_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "address");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'address\'. Supported types: {typeof(Address)}, {typeof(string)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }

    private static object[] DecodeNumberTests =
    {
        new object[] { BigInteger.One, "0x01" },
        new object[] { 1, "0x01" },
        new object[] { BigInteger.One, "0x1" },
        new object[] { 1, "0x1" },
        new object[] { BigInteger.One, "0x0000000000000000000000000000000000000000000000000000000000000001" },
        new object[] { 1, "0x0000000000000000000000000000000000000000000000000000000000000001" },
        new object[] { BigInteger.Zero, "0x0000000000" },
        new object[] { 0, "0x000000000" },
    };

    [TestCaseSource(nameof(DecodeNumberTests))]
    public void TestDecodeNumber<T>(T expected, string value)
    {
        try
        {
            T result = ABI.Decode<T>(value, "uint256");
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeNumberThrowsTests =
    {
        new object[] { Encoding.UTF8.GetBytes("banana") },
        new object[] { new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249") },
        new object[] { "SDK by Horizon" },
        new object[] { true },
    };

    [TestCaseSource(nameof(DecodeNumberThrowsTests))]
    public void TestDecodeNumber_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "uint256");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'uint256\'. Supported types: {typeof(BigInteger)}, {typeof(int)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }

    private static object[] DecodeBoolTests =
    {
        new object[] { true, "0x1" },
        new object[] { false, "0x0" },
        new object[] { true, "0x0001" },
        new object[] { false, "0x0000" },
        new object[] { true, "0x0000000000000000000000000000000000000000000000000000000000000001" },
        new object[] { false, "0x0000000000000000000000000000000000000000000000000000000000000000" },
    };

    [TestCaseSource(nameof(DecodeBoolTests))]
    public void TestDecodeBool<T>(T expected, string value)
    {
        try
        {
            T result = ABI.Decode<T>(value, "bool");
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeBoolThrowsTests =
    {
        new object[] { Encoding.UTF8.GetBytes("banana") },
        new object[] { new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249") },
        new object[] { "SDK by Horizon" },
    };

    [TestCaseSource(nameof(DecodeBoolThrowsTests))]
    public void TestDecodeBool_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "bool");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'bool\'. Supported types: {typeof(bool)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }

    private static object[] DecodeBytesTests =
    {
        new object[] { Encoding.UTF8.GetBytes("SDK by Horizon"), "SDK by Horizon" },
        new object[] { Encoding.UTF8.GetBytes(""), "" },
        new object[] { Encoding.UTF8.GetBytes(longMultiLineString), longMultiLineString },
    };

    [TestCaseSource(nameof(DecodeBytesTests))]
    public void TestDecodeBytes<T>(T expected, string value)
    {
        try
        {
            T result = ABI.Decode<T>(value, "bytes");
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeBytesThrowsTests =
    {
        new object[] { new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249") },
        new object[] { BigInteger.One },
        new object[] { 5 },
        new object[] { "SDK by Horizon" },
        new object[] { true },
    };

    [TestCaseSource(nameof(DecodeBytesThrowsTests))]
    public void TestDecodeBytes_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "bytes");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'bytes\'. Supported types: {typeof(byte[])}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }

    public static object[] DecodeFixedBytesTests =
    {
        new object[]
        {
            8, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 },
            "0x0000000000000000000000000000000000000000000000000000000000000008" + "0102030405060708"
        },
        new object[]
        {
            8, new byte[] { 0x00, 0xFF, 0xAA, 0x55, 0x99, 0x66, 0x33, 0x22 },
            "0x0000000000000000000000000000000000000000000000000000000000000008" + "00FFAA5599663322"
        },
        new object[]
        {
            8, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
            "0x0000000000000000000000000000000000000000000000000000000000000008" + "FFFFFFFFFFFFFFFF"
        },
        new object[]
        {
            16,
            new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10
            },
            "0x0000000000000000000000000000000000000000000000000000000000000010" + "0102030405060708090A0B0C0D0E0F10"
        },
        new object[] { 0, new byte[0], "0x0000000000000000000000000000000000000000000000000000000000000000" },
        new object[]
        {
            2, new byte[] { 0x01, 0x02 }, "0x0000000000000000000000000000000000000000000000000000000000000002" + "0102"
        },
        new object[]
        {
            32, new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
                0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20
            },
            "0x0000000000000000000000000000000000000000000000000000000000000020" +
            "0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20"
        },
        new object[]
        {
            8,
            new FixedByte(8, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }),
            "0x0000000000000000000000000000000000000000000000000000000000000008" + "0102030405060708"
        },
        new object[]
        {
            8,
            new FixedByte(8, new byte[] { 0x00, 0xFF, 0xAA, 0x55, 0x99, 0x66, 0x33, 0x22 }),
            "0x0000000000000000000000000000000000000000000000000000000000000008" + "00FFAA5599663322"
        },
        new object[]
        {
            8,
            new FixedByte(8, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }),
            "0x0000000000000000000000000000000000000000000000000000000000000008" + "FFFFFFFFFFFFFFFF"
        },
        new object[]
        {
            16,
            new FixedByte(16,
                new byte[]
                {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10
                }),
            "0x0000000000000000000000000000000000000000000000000000000000000010" + "0102030405060708090A0B0C0D0E0F10"
        },
        new object[]
        {
            0,
            new FixedByte(0, new byte[0]),
            "0x0000000000000000000000000000000000000000000000000000000000000000"
        },
        new object[]
        {
            2,
            new FixedByte(2, new byte[] { 0x01, 0x02 }),
            "0x0000000000000000000000000000000000000000000000000000000000000002" + "0102"
        },
        new object[]
        {
            32,
            new FixedByte(32, new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
                0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20
            }),
            "0x0000000000000000000000000000000000000000000000000000000000000020" +
            "0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20"
        },
        new object[]
        {
            64,
            new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
                0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20,
                0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28,
                0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30,
                0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38,
                0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40
            },
            "0x0000000000000000000000000000000000000000000000000000000000000040" +
            "0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F40"
        },
    };

    [TestCaseSource(nameof(DecodeFixedBytesTests))]
    public void TestDecodeFixedBytes<T>(int length, T expected, string value)
    {
        try
        {
            T result = ABI.Decode<T>(value, $"bytes{length}");
            if (typeof(T) == typeof(byte[]))
            {
                CollectionAssert.AreEqual(expected as byte[], result as byte[]);
            }
            else if (typeof(T) == typeof(FixedByte))
            {
                FixedByte expectedFB = expected as FixedByte;
                FixedByte resultFB = result as FixedByte;
                Assert.AreEqual(expectedFB.Count, resultFB.Count);
                CollectionAssert.AreEqual(expectedFB.Data, resultFB.Data);
            }
            else
            {
                Assert.Fail($"Unexpected type {typeof(T)} received");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public void TestDecodeFixedBytes_maxSizeOf32ForFixedBytes()
    {
        try
        {
            string value = "0x0000000000000000000000000000000000000000000000000000000000000021" +
                           "0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F2021";
            FixedByte result = ABI.Decode<FixedByte>(value, "bytes33");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (Exception ex)
        {
            Assert.AreEqual($"ABIByte type length should be [0,32]", ex.Message);
        }
    }

    private static object[] DecodeFixedBytesThrowsTests =
    {
        new object[] { new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249") },
        new object[] { BigInteger.One },
        new object[] { 5 },
        new object[] { "SDK by Horizon" },
        new object[] { true },
    };

    [TestCaseSource(nameof(DecodeFixedBytesThrowsTests))]
    public void TestDecodeFixedBytes_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "bytes1");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'bytes1\'. Supported types: {typeof(byte[])}, {typeof(FixedByte)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }
    
    private static object[] DecodeArrayTests =
    {
        new object[]
        {
            "0x000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000",
            new bool[] { true, false },
            "bool[]"
        },
        new object[]
        {
            "0x000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001",
            new bool[] { true, false, true },
            "bool[3]"
        },
        new object[]
        {
            "0x0000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000004",
            new BigInteger[] { 1, 2, 3, 4 },
            "uint256[4]"
        },
        new object[]
        {
            "0x00000000000000000000000000000000000000000000000000000000000000040000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000004",
            new BigInteger[] { 1, 2, 3, 4 },
            "uint256[]"
        },
        new object[]
        {
            "0x00000000000000000000000000000000000000000000000000000000000000010000000000000000000000001099542D7dFaF6757527146C0aB9E70A967f71C0",
            new Address[] { new Address(SequenceCoder.AddressChecksum("0x1099542D7dFaF6757527146C0aB9E70A967f71C0")) },
            "address[]"
        },
        new object[]
        {
            "0x000000000000000000000000c683a014955b75F5ECF991d4502427c8fa1Aa2490000000000000000000000000000000000000000000000000000000000000000",
            new Address[] { new Address(SequenceCoder.AddressChecksum("0xc683a014955b75F5ECF991d4502427c8fa1Aa249")), Address.ZeroAddress, },
            "address[2]"
        },
    };

    [TestCaseSource(nameof(DecodeArrayTests))]
    public void TestDecodeArray<T>(string value, T expected, string evmType)
    {
        try
        {
            T result = ABI.Decode<T>(value, evmType);
            if (typeof(T) == typeof(Address[]))
            {
                var resultArray = result as Address[];
                var expectedArray = expected as Address[];
                int length = resultArray.Length;
                Assert.AreEqual(expectedArray.Length, length);
                for (int i = 0; i < length; i++)
                {
                    Assert.AreEqual(expectedArray[i].ToString(), resultArray[i].ToString());
                }
            }
            else
            {
                CollectionAssert.AreEqual(expected as IEnumerable, result as IEnumerable);
            }
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeArrayThrowsTests =
    {
        new object[] { new Address("0xc683a014955B75F5ECF991D4502427C8FA1AA249") },
        new object[] { BigInteger.One },
        new object[] { 5 },
        new object[] { "SDK by Horizon" },
        new object[] { true },
    };

    [TestCaseSource(nameof(DecodeArrayThrowsTests))]
    public void TestDecodeArray_throwsOnInvalidType<T>(T type)
    {
        try
        {
            var result = ABI.Decode<T>("0x123", "uint256[]");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'uint256[]\'. Supported types: {typeof(IEnumerable)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
        try
        {
            var result = ABI.Decode<T>("0x123", "uint256[5]");
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'uint256[5]\'. Supported types: {typeof(IEnumerable)}",
                ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }
    
    private static object[] DecodeDynamicArrayTests =
    {
        new object[]
        {
            "0x00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000000a000000000000000000000000000000000000000000000000000000000000000e0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
            new string[] { "", "", "" },
        },
        new object[]
        {
            "0x0000000000000000000000000000000000000000000000000000000000000020"+
            "0000000000000000000000000000000000000000000000000000000000000004"+
            "0000000000000000000000000000000000000000000000000000000000000080"+
            "00000000000000000000000000000000000000000000000000000000000000c0"+
            "0000000000000000000000000000000000000000000000000000000000000100"+
            "0000000000000000000000000000000000000000000000000000000000000140"+
            "000000000000000000000000000000000000000000000000000000000000000e"+ 
            "53444b20627920486f72697a6f6e000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000004"+ 
            "776f726400000000000000000000000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000000"+ 
            "0000000000000000000000000000000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000114"+ 
            "7468697320697320610a6d756c74692d6c696e6520737472696e670a7468617420697320616c736f20726174686572206c6f6e6720616e6420706f696e746c6573730a6275742069662049206a757374206b65657020747970696e67206d617962652069742077696c6c206d616b6520666f7220616e20696e746572657374696e67207465737420636173650a616e64206d617962652c0a6a757374206d617962652c0a796f752061732061207265616465722077696c6c20646576656c6f7020616e206576657220736f20736c6967687420736d696c65206f6e20796f757220666163650a616e6420696e207468617420636173652c206d79206d697373696f6e2077696c6c20626520636f6d706c6574652e000000000000000000000000",
            new string[] { "SDK by Horizon", "word", "", DecodeABITests.longMultiLineString},
        },
        new object[]
        {
            "0x0000000000000000000000000000000000000000000000000000000000000020"+
            "0000000000000000000000000000000000000000000000000000000000000005"+
            "00000000000000000000000000000000000000000000000000000000000000a0"+
            "00000000000000000000000000000000000000000000000000000000000000e0"+
            "0000000000000000000000000000000000000000000000000000000000000120"+
            "0000000000000000000000000000000000000000000000000000000000000160"+
            "00000000000000000000000000000000000000000000000000000000000002a0"+
            "000000000000000000000000000000000000000000000000000000000000000e"+ 
            "53444b20627920486f72697a6f6e000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000004"+
            "776f726400000000000000000000000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000114"+
            "7468697320697320610a6d756c74692d6c696e6520737472696e670a7468617420697320616c736f20726174686572206c6f6e6720616e6420706f696e746c6573730a6275742069662049206a757374206b65657020747970696e67206d617962652069742077696c6c206d616b6520666f7220616e20696e746572657374696e67207465737420636173650a616e64206d617962652c0a6a757374206d617962652c0a796f752061732061207265616465722077696c6c20646576656c6f7020616e206576657220736f20736c6967687420736d696c65206f6e20796f757220666163650a616e6420696e207468617420636173652c206d79206d697373696f6e2077696c6c20626520636f6d706c6574652e000000000000000000000000"+
            "0000000000000000000000000000000000000000000000000000000000000009"+
            "2048757272617921200000000000000000000000000000000000000000000000",
            new string[] { "SDK by Horizon", "word", "", DecodeABITests.longMultiLineString, " Hurray! ",},
        },
        new object[]
        {
            "0x000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000e53444b20627920486f72697a6f6e000000000000000000000000000000000000",
            new string[] { "SDK by Horizon" },
        },
    };

    [TestCaseSource(nameof(DecodeDynamicArrayTests))]
    public void TestDecodeDynamicArray<T>(string value, T expected)
    {
        try
        {
            T result = ABI.Decode<T>(value, "(string[])");

            CollectionAssert.AreEqual(expected as IEnumerable, result as IEnumerable);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeNestedArrayTests_twoLayer =
    {
        new object[]
        {
            "0x00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000001200000000000000000000000000000000000000000000000000000000000000140000000000000000000000000000000000000000000000000000000000000000500000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000000000000000000000000000000000000040000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000",
            new []
            {
                new object[] { (BigInteger)1, (BigInteger)2, (BigInteger)3, (BigInteger)4, (BigInteger)5 },
                new object[] { },
                new object[] { BigInteger.Zero, },
            },
            "(uint[][])",
        },
    };

    [TestCaseSource(nameof(DecodeNestedArrayTests_twoLayer))]
    public void TestDecodeNestedArray_twoLayer(string value, object[][] expected, string evmType)
    {
        try
        {
            object[][] result = ABI.Decode<object[][]>(value, evmType);

            Assert.AreEqual(expected.Length, result.Length, "Arrays have different lengths.");

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Length, result[i].Length, $"Nested array {i} has different length.");

                for (int j = 0; j < expected[i].Length; j++)
                {
                    Assert.AreEqual(expected[i][j], result[i][j], $"Element at position [{i}][{j}] is different.");
                }
            }
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    private static object[] DecodeNestedArrayTests_threeLayer =
    {
        new object[]
        {
            "0x00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000001600000000000000000000000000000000000000000000000000000000000000180000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000bb80000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000001200000000000000000000000000000000000000000000000000000000000000140000000000000000000000000000000000000000000000000000000000000000500000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000000000000000000000000000000000000040000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000",
            new object[][][]
            {
                new object[][]
                {
                    new object[] { (BigInteger)1, (BigInteger)2, (BigInteger)3000, (BigInteger)4, (BigInteger)5 }
                },
                new object[][] { },
                new[]
                {
                    new object[] { (BigInteger)1, (BigInteger)2, (BigInteger)3, (BigInteger)4, (BigInteger)5 },
                    new object[] { },
                    new object[] { BigInteger.Zero }
                }
            },
            "(uint[][][])"
        }
    };

    [TestCaseSource(nameof(DecodeNestedArrayTests_threeLayer))]
    public void TestDecodeNestedArray_threeLayer(string value, object[][] expected, string evmType)
    {
        try
        {
            object[][] result = ABI.Decode<object[][]>(value, evmType);
            
            Assert.AreEqual(expected.Length, result.Length, "Array dimensions do not match.");

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Length, result[i].Length, $"Array dimensions do not match at index {i}.");

                for (int j = 0; j < expected[i].Length; j++)
                {
                    if (expected[i][j] is Array expectedij && result[i][j] is Array resultij)
                    {
                        Assert.AreEqual(expectedij.Length, resultij.Length, $"Array dimensions do not match at index {i},{j}.");

                        for (int k = 0; k < expectedij.Length; k++)
                        {
                            Assert.AreEqual(expectedij.GetValue(k), resultij.GetValue(k), $"Values do not match at index {i},{j},{k}.");
                        }
                    }
                    else
                    {
                        Assert.Fail("Expected 3-layer array but didn't get one");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }
}