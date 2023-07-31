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
        
        toEncode = new string[] { "SDK by Horizon", "word", "", DecodeABITests.longMultiLineString, " Hurray! "};
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode)}]");
        result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode)).ToHex();
        Debug.Log(result);
        
        toEncode = new string[] { "", "", "" };
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode)}]");
        result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode)).ToHex();
        Debug.Log(result);
        
        toEncode = new string[] { "SDK by Horizon"};
        
        Debug.Log($"Encoding string array: [{string.Join(", ", toEncode)}]");
        result = abiEncode.GetABIEncoded(new ABIValue("string[]", toEncode)).ToHex();
        Debug.Log(result);

        string longString = DecodeABITests.longMultiLineString;
        Debug.Log($"Encoding string: {longString}");
        result = abiEncode.GetABIEncoded(new ABIValue("string", longString)).ToHex();
        Debug.Log(result);
    }
}
