using System.Collections.Generic;
using NUnit.Framework;
using Sequence.ABI;
using System.Numerics;
using UnityEngine;
using System.Text;

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
            string abi_encoded_data = ABI.Pack("sam(bytes,bool,uint256[])", "dave", true, new List<int>() { 1, 2, 3});
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }


        //Dynamic types:

        {
            string expected_data = "0x8be6524600000000000000000000000000000000000000000000000000000000000001230000000000000000000000000000000000000000000000000000000000000080313233343536373839300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000e0000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000004560000000000000000000000000000000000000000000000000000000000000789000000000000000000000000000000000000000000000000000000000000000d48656c6c6f2c20776f726c642100000000000000000000000000000000000000";
            string abi_encoded_data = ABI.Pack("f(uint256,uint32[],bytes10,bytes)", 0x123,new List<int>(){ 0x456, 0x789 }, new FixedByte(10,"1234567890"), "Hello, world!");
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }
        

        {
            //the function signature in the example is calculated wrong
            string expected_data = "0xd7bd679a000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000001400000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000000a000000000000000000000000000000000000000000000000000000000000000e000000000000000000000000000000000000000000000000000000000000000036f6e650000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000374776f000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000057468726565000000000000000000000000000000000000000000000000000000";
            string abi_encoded_data = ABI.Pack("g(string[],uint256[][])", new List<List<int>> { new List<int> { 1,2}, new List<int> { 3} }, new List<string> { "one", "two", "three"});
            CollectionAssert.AreEqual(expected_data, abi_encoded_data);
        }

    }


}
