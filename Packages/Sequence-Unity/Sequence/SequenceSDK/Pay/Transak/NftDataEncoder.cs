using System;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Pay.Transak
{
    public class NftDataEncoder
    {
        public static string Encode(TransakNftData item)
        {
            string itemJson = JsonConvert.SerializeObject(new [] { item });
            string itemJsonBase64 = itemJson.StringToBase64();
            return Uri.EscapeDataString(itemJsonBase64);
        }
    }
}