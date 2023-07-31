using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.Encoders;
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
        result = abiEncode.GetABIEncoded(new ABIValue("string[][]", nestedBigIntegerArray)).ToHex();
        Debug.Log(result);
    }
}
