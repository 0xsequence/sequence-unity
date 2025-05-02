using System;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace Sequence.Pay.Transak
{
    public static class CompressionUtility
    {
        private static byte[] Deflate(string data)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            using var outputStream = new MemoryStream();
            using (var deflater = new ZlibStream(outputStream, CompressionMode.Compress, CompressionLevel.Default))
            {
                deflater.Write(inputBytes, 0, inputBytes.Length);
            }
            return outputStream.ToArray();
        }

        public static string DeflateAndEncodeToBase64(string data)
        {
            byte[] compressedData = Deflate(data);
            return Base64UrlEncode(compressedData);
        }
        
        private static string Base64UrlEncode(byte[] input)
        {
            string base64 = Convert.ToBase64String(input);
            return base64;
        }
    }
}