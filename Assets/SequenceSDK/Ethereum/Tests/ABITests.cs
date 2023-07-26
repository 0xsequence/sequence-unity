using System.Collections.Generic;
using NUnit.Framework;
using Sequence.ABI;
using System.Numerics;
using UnityEngine;
using System.Text;
using Sequence.Extensions;
using System;
using Sequence;
using Sequence.Contracts;
using UnityEngine.TestTools;

public class ABITests
{
    AddressCoder _addressCoder = new AddressCoder();
    ArrayCoder _arrayCoder = new ArrayCoder();
    BooleanCoder _booleanCoder = new BooleanCoder();
    BytesCoder _bytesCoder = new BytesCoder();
    //FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
    NumberCoder _numberCoder = new NumberCoder();
    StringCoder _stringCoder = new StringCoder();


    [Test]
    public void Address1Encoding()
    {
        string parameter = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000008e3e38fe7367dd3b52d1e281e4e8400447c8d8b9");
        byte[] encoded = _addressCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);

        //TODO Case: Address length surpasses the maximum allowed limit
        //TODO Case: Address length insufficient
        //TODO Case: Address

    }
    [Test]
    public void Address1Decoding()
    {
        string parameter = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        byte[] encoded = _addressCoder.Encode(parameter);
        string decoded = (string)_addressCoder.Decode(encoded);
        CollectionAssert.AreEqual(decoded, parameter);
    }

    [Test]
    public void Address2Encoding()
    {
        //Param ContractS Address
        string parameter = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
        byte[] expected = SequenceCoder.HexStringToByteArray("000000000000000000000000fcfde38a1eeae0ee7e130bbf66e94844bc5d5b6b");
        byte[] encoded = _addressCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);
    }

    [Test]
    public void Address2Decoding()
    {
        string parameter = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
        byte[] encoded = _addressCoder.Encode(parameter);

        string decoded = (string)_addressCoder.Decode(encoded);
        CollectionAssert.AreEqual(decoded, parameter);
    }

    [Test]
    public void DynamicArrayEncoding()
    {


        List<BigInteger> parameter = new List<BigInteger> { 1, 2, 3 };
        byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000003");

        byte[] encoded = _arrayCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);


    }

    [Test]
    public void DynamicArrayDecoding()
    {
        List<BigInteger> parameter = new List<BigInteger> { 1, 2, 3 };
        byte[] encoded = _arrayCoder.Encode(parameter);


        List<object> types = new List<object> { new List<object> { ABIType.NUMBER, ABIType.NUMBER, ABIType.NUMBER } };
        List<object> decodedRaw = _arrayCoder.Decode(encoded, types);
        List<object> decodedBigInt = (List<object>)(decodedRaw[0]);
        List<BigInteger> decoded = new List<BigInteger>();

        foreach (BigInteger d in decodedBigInt)
        {
            decoded.Add(d);
        }
        CollectionAssert.AreEqual(decoded, parameter);
    }



    [Test]
    public void BooleanTrueEncoding()
    {

        //Encode
        //Param True
        bool parameter = true;
        byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000001");
        byte[] encoded = _booleanCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);

    }

    [Test]
    public void BooleanTrueDecoding()
    {
        bool parameter = true;
        byte[] encoded = _booleanCoder.Encode(parameter);

        bool decoded = (bool)_booleanCoder.Decode(encoded);
        Assert.AreEqual(decoded, parameter);
    }

    [Test]
    public void BooleanFalseEncoding()
    {
        bool parameter = false;
        byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000000");
        byte[] encoded = _booleanCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);
    }

    [Test]
    public void BooleanFalseDecoding()
    {
        bool parameter = false;
        byte[] encoded = _booleanCoder.Encode(parameter);

        bool decoded = (bool)_booleanCoder.Decode(encoded);
        Assert.AreEqual(decoded, parameter);
    }

    [Test]
    public void BytesEncoding()
    {
        //Encode
        byte[] parameter = SequenceCoder.HexStringToByteArray("0xaabbccdd"); 
        byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000004aabbccdd00000000000000000000000000000000000000000000000000000000");

        byte[] encoded = _bytesCoder.Encode(parameter);

        CollectionAssert.AreEqual(expected, encoded);

        
    }

    [Test]
    public void BytesDecoding()
    {
        byte[] parameter = SequenceCoder.HexStringToByteArray("0xaabbccdd");
        byte[] encoded = _bytesCoder.Encode(parameter);
        //Decode
        byte[] decoded = (byte[])_bytesCoder.Decode(encoded);

        CollectionAssert.AreEqual(decoded, parameter);
    }


    [Test]
    public void PositiveIntEncoding()
    {

            BigInteger parameter = 5;
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000005");
            byte[] encoded = _numberCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

        //TODO Case: unit8, unit32, unit40 and unit256
    }

    [Test]
    public void PositiveIntDecoding()
    {
        BigInteger parameter = 5;
        byte[] encoded = _numberCoder.Encode(parameter);
        BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
        Assert.AreEqual(decoded, parameter);
    }

    [Test]
    public void NegativeIntEncoding()
    {
        BigInteger parameter = -5;
        byte[] expected = SequenceCoder.HexStringToByteArray("fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffb");
        byte[] encoded = _numberCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);
    }


    [Test]
    public void NegativeIntDecoding()
    {
        BigInteger parameter = -5;
        byte[] encoded = _numberCoder.Encode(parameter);
        BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
        Assert.AreEqual(decoded, parameter);
    }

    [Test]
    public void StringEncoding()
    {
        {
            //Param string
            string parameter = "sequence";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000873657175656e6365000000000000000000000000000000000000000000000000");
            byte[] encoded = _stringCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            
        }
    }

    [Test]
    public void StringDecoding()
    {
        string parameter = "sequence";
        byte[] encoded = _stringCoder.Encode(parameter);

        string decoded = (string)_stringCoder.Decode(encoded);
        Assert.AreEqual(decoded, parameter);
    }

    [Test]
    public void FloatEncoding()
    {
        //fixed point
    }

    [Test]
    public void FloatDecoding()
    {
        //fixed point
    }


    /*
     * Function examples are taken from 
     * https://docs.soliditylang.org/en/develop/abi-spec.html#function-selector
     */

    [Test]
    public void FunctionSelector1Hashing()
    {
        string methodId = ABI.FunctionSelector("baz(uint32,bool)");
        string expected = "0xcdcd77c0";
        Assert.AreEqual(methodId, expected);
    }

    [Test]
    public void FunctionSelector2Hashing()
    {
        string methodId = ABI.FunctionSelector("sam(bytes,bool,uint256[])");
        string expected = "0xa5643bf2";
        Assert.AreEqual(methodId, expected);
    }

    [Test]
    public void FunctionSelector3Hashing()
    {
        string methodId = ABI.FunctionSelector("f(uint256,uint32[],bytes10,bytes)");
        string expected = "0x8be65246";
        Assert.AreEqual(methodId, expected);
    }

    //ABI Pack
    [Test]
    public void ABIPackTests()
    {
        string expected_data = "0x00fdd58e0000000000000000000000006615e4e985bf0d137196897dfa182dbd7127f54f0000000000000000000000000000000000000000000000000000000000000002";
        Debug.Log("?");
        string encoded_data =ABI.Pack("balanceOf(address,uint256)", "0x6615e4e985bf0d137196897dfa182dbd7127f54f", 2);
        Debug.Log("encoded packed data:" + encoded_data);
        CollectionAssert.AreEqual(expected_data, encoded_data);


    }

    

    [Test]
    public void ExampleContractTests()
    {
        //Examples from https://docs.soliditylang.org/en/v0.8.19/abi-spec.html#examples
        //
        {

            //function baz(uint32 x, bool y) public pure returns (bool r) { r = x > 32 || y; }
            string expected_data = "0xcdcd77c000000000000000000000000000000000000000000000000000000000000000450000000000000000000000000000000000000000000000000000000000000001";
            string abi_encoded_data = ABI.Pack("baz(uint32,bool)", 69, true);
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);

        }
        {
            string expected_data = "0xfce353f661626300000000000000000000000000000000000000000000000000000000006465660000000000000000000000000000000000000000000000000000000000";
            string abi_encoded_data = ABI.Pack("bar(bytes3[2])", new FixedByte[2] { new FixedByte(3, "abc"), new FixedByte(3, "def") });
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }
        {
            string expected_data = "0xa5643bf20000000000000000000000000000000000000000000000000000000000000060000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000000464617665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000003";
            string abi_encoded_data = ABI.Pack("sam(bytes,bool,uint256[])", "dave".ToByteArray(), true, new List<int>() { 1, 2, 3});
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }
        {
            string expected_data = "0xa5643bf20000000000000000000000000000000000000000000000000000000000000060000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000000464617665000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000003";
            string abi_encoded_data = ABI.Pack("sam(bytes,bool,uint256[])", "dave".ToByteArray(), true, new BigInteger[] { 1, 2, 3 });
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }


        //Dynamic types:

        {
            string expected_data = "0x8be6524600000000000000000000000000000000000000000000000000000000000001230000000000000000000000000000000000000000000000000000000000000080313233343536373839300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000e0000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000004560000000000000000000000000000000000000000000000000000000000000789000000000000000000000000000000000000000000000000000000000000000d48656c6c6f2c20776f726c642100000000000000000000000000000000000000";
            string abi_encoded_data = ABI.Pack("f(uint256,uint32[],bytes10,bytes)", 0x123,new List<int>(){ 0x456, 0x789 }, new FixedByte(10,"1234567890"), "Hello, world!".ToByteArray());
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }
        

        {
            //the function signature in the example is calculated wrong
            string expected_data = "0xd7bd679a000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000001400000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000000a000000000000000000000000000000000000000000000000000000000000000e000000000000000000000000000000000000000000000000000000000000000036f6e650000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000374776f000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000057468726565000000000000000000000000000000000000000000000000000000";
            string abi_encoded_data = ABI.Pack("g(string[],uint256[][])", new List<List<int>> { new List<int> { 1,2}, new List<int> { 3} }, new List<string> { "one", "two", "three"});
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }

        // Test exception thrown when improper types provided
        try
        {
            string abi_encoded_data = ABI.Pack("sam(bytes,bool,uint256[])", "dave", true, new List<int>() { 1, 2, 3 });
            Assert.Fail("Expected exception but none was thrown");
        }
        catch (Exception ex)
        {
            Assert.AreEqual("Error packing data: Argument type is not as expected. Expected: BYTES Received: STRING", ex.Message);
        }

    }
    
    private static object[] DecodeAbiTests = {
        new object[]
        {
            ERC20.Abi, new Dictionary<string, List<(string[], string)>>
            {
                { "name", new List<(string[], string)> { (new string[] { }, "string") } },
                { "allowance", new List<(string[], string)> { (new string[] { "address", "address" }, "uint256") } },
                { "approve", new List<(string[], string)> { (new string[] { "address", "uint256" }, "bool") } },
                { "balanceOf", new List<(string[], string)> { (new string[] { "address" }, "uint256") } },
                { "burn", new List<(string[], string)> { (new string[] { "uint256" }, null) } },
                { "burnFrom", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "decimals", new List<(string[], string)> { (new string[] { }, "uint8") } },
                { "decreaseAllowance", new List<(string[], string)> { (new string[] { "address", "uint256" }, "bool") } },
                { "increaseAllowance", new List<(string[], string)> { (new string[] { "address", "uint256" }, "bool") } },
                { "mint", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "owner", new List<(string[], string)> { (new string[] { }, "address") } },
                { "renounceOwnership", new List<(string[], string)> { (new string[] { }, null) } },
                { "symbol", new List<(string[], string)> { (new string[] { }, "string") } },
                { "totalSupply", new List<(string[], string)> { (new string[] { }, "uint256") } },
                { "transfer", new List<(string[], string)> { (new string[] { "address", "uint256" }, "bool") } },
                { "transferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, "bool") } },
                { "transferOwnership", new List<(string[], string)> { (new string[] { "address" }, null) } },
            },
        },
        new object[]
        {
            ERC721.Abi, new Dictionary<string, List<(string[], string)>>
            {
                { "approve", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "balanceOf", new List<(string[], string)> { (new string[] { "address" }, "uint256") } },
                { "burn", new List<(string[], string)> { (new string[] { "uint256" }, null) } },
                { "getApproved", new List<(string[], string)> { (new string[] { "uint256" }, "address") } },
                { "isApprovedForAll", new List<(string[], string)> { (new string[] { "address", "address" }, "bool") } },
                { "name", new List<(string[], string)> { (new string[] { }, "string") } },
                { "owner", new List<(string[], string)> { (new string[] { }, "address") } },
                { "ownerOf", new List<(string[], string)> { (new string[] { "uint256" }, "address") } },
                { "renounceOwnership", new List<(string[], string)> { (new string[] { }, null) } },
                { "safeMint", new List<(string[], string)> { (new string[] { "address", "uint256" }, null) } },
                { "safeTransferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, null), (new string[] { "address", "address", "uint256", "bytes" }, null) } },
                { "setApprovalForAll", new List<(string[], string)> { (new string[] { "address", "bool" }, null) } },
                { "supportsInterface", new List<(string[], string)> { (new string[] { "bytes4" }, "bool") } },
                { "symbol", new List<(string[], string)> { (new string[] { }, "string") } },
                { "tokenURI", new List<(string[], string)> { (new string[] { "uint256" }, "string") } },
                { "transferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256" }, null) } },
                { "transferOwnership", new List<(string[], string)> { (new string[] { "address" }, null) } },
            },
        },
        new object[]
        {
            ERC1155.Abi, new Dictionary<string, List<(string[], string)>>
            {
                { "balanceOf", new List<(string[], string)> { (new string[] { "address", "uint256" }, "uint256") } },
                { "balanceOfBatch", new List<(string[], string)> { (new string[] { "address[]", "uint256[]" }, "uint256[]") } },
                { "burn", new List<(string[], string)> { (new string[] { "address", "uint256", "uint256" }, null) } },
                { "burnBatch", new List<(string[], string)> { (new string[] { "address", "uint256[]", "uint256[]" }, null) } },
                { "exists", new List<(string[], string)> { (new string[] { "uint256" }, "bool") } },
                { "isApprovedForAll", new List<(string[], string)> { (new string[] { "address", "address" }, "bool") } },
                { "mint", new List<(string[], string)> { (new string[] { "address", "uint256", "uint256", "bytes" }, null) } },
                { "mintBatch", new List<(string[], string)> { (new string[] { "address", "uint256[]", "uint256[]", "bytes" }, null) } },
                { "setApprovalForAll", new List<(string[], string)> { (new string[] { "address", "bool" }, null) } },
                { "setURI", new List<(string[], string)> { (new string[] { "string" }, null) } },
                { "supportsInterface", new List<(string[], string)> { (new string[] { "bytes4" }, "bool") } },
                { "totalSupply", new List<(string[], string)> { (new string[] { "uint256" }, "uint256") } },
                { "transferOwnership", new List<(string[], string)> { (new string[] { "address" }, null) } },
                { "uri", new List<(string[], string)> { (new string[] { "uint256" }, "string") } },
                { "owner", new List<(string[], string)> { (new string[] { }, "address") } },
                { "renounceOwnership", new List<(string[], string)> { (new string[] { }, null) } },
                { "safeTransferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256", "uint256", "bytes" }, null) } },
                { "safeBatchTransferFrom", new List<(string[], string)> { (new string[] { "address", "address", "uint256[]", "uint256[]", "bytes" }, null) } },
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

    private static string longMultiLineString = @"this is a
multi-line string
that is also rather long and pointless
but if I just keep typing maybe it will make for an interesting test case
and maybe,
just maybe,
you as a reader will develop an ever so slight smile on your face
and in that case, my mission will be complete.";

    private static object[] DecodeStringTests =
    {
        new string[] { "banana", "0x62616e616e61" },
        new string[] {"welcome to Horizon", "0x77656c636f6d6520746f20486f72697a6f6e"},
        new string[] { longMultiLineString, "0x7468697320697320610d0a6d756c74692d6c696e6520737472696e670d0a7468617420697320616c736f20726174686572206c6f6e6720616e6420706f696e746c6573730d0a6275742069662049206a757374206b65657020747970696e67206d617962652069742077696c6c206d616b6520666f7220616e20696e746572657374696e67207465737420636173650d0a616e64206d617962652c0d0a6a757374206d617962652c0d0a796f752061732061207265616465722077696c6c20646576656c6f7020616e206576657220736f20736c6967687420736d696c65206f6e20796f757220666163650d0a616e6420696e207468617420636173652c206d79206d697373696f6e2077696c6c20626520636f6d706c6574652e"},
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
        new object[] {true},
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
            Assert.AreEqual($"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'string\'. Supported types: {typeof(string)}", ex.Message);
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
        new object[] {true},
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
            Assert.AreEqual($"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'address\'. Supported types: {typeof(Address)}, {typeof(string)}", ex.Message);
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
        new object[] {"SDK by Horizon"},
        new object[] {true},
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
            Assert.AreEqual($"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'uint256\'. Supported types: {typeof(BigInteger)}, {typeof(int)}", ex.Message);
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
        new object[] {"SDK by Horizon"},
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
            Assert.AreEqual($"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'bool\'. Supported types: {typeof(bool)}", ex.Message);
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
        new object[] { Encoding.UTF8.GetBytes(longMultiLineString), longMultiLineString},
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
        new object[] {"SDK by Horizon"},
        new object[] {true},
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
            Assert.AreEqual($"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'bytes\'. Supported types: {typeof(byte[])}", ex.Message);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected ArgumentException, but got: " + ex.GetType());
        }
    }

}
