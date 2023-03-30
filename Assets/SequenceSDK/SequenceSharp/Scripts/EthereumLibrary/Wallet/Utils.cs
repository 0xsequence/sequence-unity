using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static string RecoverAddress(object message, byte[] signature)
    {
        throw new System.NotImplementedException();
    }

    //TODO : find all possible types for this digest
    public static string RecoverAddressFromDigest(object digest, byte[] signature)
    {
        //check length of digest..
        throw new System.NotImplementedException();
        //if (signature.Length != 65) return "";//todo: find common address

    }
    
}
