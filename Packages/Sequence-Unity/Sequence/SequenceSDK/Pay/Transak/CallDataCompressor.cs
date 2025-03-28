using System;

namespace Sequence.Pay.Transak
{
    public class CallDataCompressor
    {
        public static string Compress(string callData)
        {
            return Uri.EscapeDataString(CompressionUtility.DeflateAndEncodeToBase64(callData));
        }
    }
}