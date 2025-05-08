using System;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine; // Make sure this is included for logging

namespace Sequence.Pay.Transak
{
    public class NftDataEncoder
    {
        public static string Encode(TransakNftData item)
        {
            string itemJson = JsonConvert.SerializeObject(new[] { item });
            Debug.Log("JSON Serialized:\n" + itemJson);

            string itemJsonBase64 = itemJson.StringToBase64();
            Debug.Log("Base64 Encoded (with padding):\n" + itemJsonBase64);

            string encoded = Uri.EscapeDataString(itemJsonBase64);
            Debug.Log("URL Encoded:\n" + encoded);

            return encoded;
        }
    }
}
