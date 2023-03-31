using System.Collections.Generic;
using NUnit.Framework;
using SequenceSharp.ABI;
using System.Numerics;

public class CoderTests
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
    public void ArrayEncoding()
    {


        //Encode 

        List<BigInteger> parameter = new List<BigInteger> { 1, 2, 3 };
        byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000003");

        byte[] encoded = _arrayCoder.Encode(parameter);
        CollectionAssert.AreEqual(expected, encoded);

        //Decode


    }

    [Test]
    public void ArrayDecoding()
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



        {
            //Param Positive


            //Decode

        }
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


    /*
     * Function examples are taken from 
     * https://docs.soliditylang.org/en/develop/abi-spec.html#function-selector
     */

    [Test]
    public void FunctionSelector1Hashing()
    {
        string methodId = SequenceCoder.FunctionSelector("baz(uint32,bool)");
        string expected = "0xcdcd77c0";
        Assert.AreEqual(methodId, expected);
    }

    [Test]
    public void FunctionSelector2Hashing()
    {
        string methodId = SequenceCoder.FunctionSelector("sam(bytes,bool,uint256[])");
        string expected = "0xa5643bf2";
        Assert.AreEqual(methodId, expected);
    }

    [Test]
    public void FunctionSelector3Hashing()
    {
        string methodId = SequenceCoder.FunctionSelector("f(uint256,uint32[],bytes10,bytes)");
        string expected = "0x8be65246";
        Assert.AreEqual(methodId, expected);
    }


}
