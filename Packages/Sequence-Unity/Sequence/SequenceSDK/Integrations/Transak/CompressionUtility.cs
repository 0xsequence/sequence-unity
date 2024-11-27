using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Sequence.Integrations.Transak
{
    public static class CompressionUtility
    {
        public static byte[] Deflate(string data)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);

            using var outputStream = new MemoryStream();
            using (var deflateStream = new DeflateStream(outputStream, CompressionLevel.Optimal))
            {
                deflateStream.Write(inputBytes, 0, inputBytes.Length);
            }

            return outputStream.ToArray();
        }

        public static string DeflateAndEncodeToBase64(string data)
        {
            byte[] compressedData = Deflate(data);
            return Convert.ToBase64String(compressedData);
        }
    }
}