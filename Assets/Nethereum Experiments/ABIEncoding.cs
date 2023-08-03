using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.Encoders;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;

public class ABIEncoding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var abiEncode = new ABIEncode();
        string[] toEncode = new string[] { "SDK by Horizon", "word", "", DecodeABITests.longMultiLineString};
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode)}]");
        var result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode)).ToHex();
        Debug.Log(result);
        
        string[] toEncode2 = new string[] { "SDK by Horizon", "word", "", DecodeABITests.longMultiLineString, " Hurray! "};
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode2)}]");
        result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode2)).ToHex();
        Debug.Log(result);
        
        string[] toEncode3 = new string[] { "", "", "" };
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode3)}]");
        result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode3)).ToHex();
        Debug.Log(result);
        
        string[] toEncode4 = new string[] { "SDK by Horizon"};
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode4)}]");
        result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode4)).ToHex();
        Debug.Log(result);

        string longString = DecodeABITests.longMultiLineString;
        Debug.Log($"Encoding string: {longString}");
        result = abiEncode.GetABIEncoded(new ABIValue("string", longString)).ToHex();
        Debug.Log(result);

        BigInteger[][] nestedBigIntegerArray =
        {
            new BigInteger[] { 1, 2, 3, 4, 5 },
            new BigInteger[] { },
            new BigInteger[] { BigInteger.Zero, },
        };
        Debug.Log("Encoding nested big integer array");
        result = abiEncode.GetABIEncoded(new ABIValue("uint256[][]", nestedBigIntegerArray)).ToHex();
        Debug.Log(result);
        
        string[][] nestedStringArray =
        {
            toEncode,
            toEncode2,
            toEncode3,
            toEncode4,
            new string[] { },
            new string[] { longString, "SDK by Horizon" },
        };
        Debug.Log("Encoding nested string array");
        result = abiEncode.GetABIEncoded(new ABIValue("string[][]", nestedStringArray)).ToHex();
        Debug.Log(result);
        
        BigInteger[][][] nestedBigIntegerArray2 =
        { 
            new BigInteger[][] {
                new BigInteger[] { 1, 2, 3000, 4, 5 },
            },
            new BigInteger[][]{},
            nestedBigIntegerArray,
        };
        Debug.Log("Encoding nested big integer array");
        result = abiEncode.GetABIEncoded(new ABIValue("uint256[][][]", nestedBigIntegerArray2)).ToHex();
        Debug.Log(result);

        Tuple<string, BigInteger[], BigInteger> testTuple =
            new Tuple<string, BigInteger[], BigInteger>("SDK by Horizon", new BigInteger[] { 1, 2, 3 },
                BigInteger.Zero);
        Debug.Log("Encoding tuple "+ testTuple);
        TupleTypeEncoder tupleEncoder = new TupleTypeEncoder();
        string encodedTuple = tupleEncoder.Encode(testTuple).ToHex();
        Debug.Log(encodedTuple); // yields empty string
        
        // Not supported
        // result = abiEncode.GetABIEncoded(new ABIValue("(string, uint256[], uint256)", testTuple)).ToHex();
        // Debug.Log(result);

        var testTuple2 = new MyTuple()
            { Item1 = "SDK by Horizon", Item2 = new BigInteger[] { 1, 2, 3 }, Item3 = BigInteger.Zero };
        result = abiEncode.GetABIEncoded(testTuple2).ToHex();
        Debug.Log(result); // yields empty string
        result = abiEncode.GetABIParamsEncoded(testTuple2).ToHex();
        Debug.Log(result);
        
        BigInteger expectedNumber = 5;
        string expectedWord = "SDK by Horizon";
        BigInteger[] expectedNumbers = new BigInteger[] { 1, 2, 3, 4, 5 };
        string[][] expectedDoubleNestedWords = new string[][]
        {
            new string[] { },
            new string[]
                { "SDK by Horizon", "", "Hello World!", DecodeABITests.longMultiLineString, "", "SDK by Horizon" },
            new string[] { "", "", "" },
            new string[] { "" }
        };

        result = abiEncode.GetABIEncoded(new ABIValue("uint256", expectedNumber), new ABIValue("string", expectedWord),
            new ABIValue("uint256[]", expectedNumbers), new ABIValue("string[][]", expectedDoubleNestedWords)).ToHex();
        
        Debug.Log("Encoding function call to testPart1");
        Debug.Log(result);
    }

    [FunctionOutput]
    public class MyTuple
    {
        [Parameter("string", 1)]
        public string Item1 { get; set; }

        [Parameter("uint256[]", 2)]
        public BigInteger[] Item2 { get; set; }

        [Parameter("uint256", 3)]
        public BigInteger Item3 { get; set; }
    }
}
