using System;

namespace Sequence.Integrations.Transak
{
    public class CallDataCompressor
    {
        public static string Compress(string callData)
        {
            return Uri.EscapeDataString(CompressionUtility.DeflateAndEncodeToBase64(callData));
        }
    }
}