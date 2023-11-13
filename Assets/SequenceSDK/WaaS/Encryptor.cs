using System.IO;
using System.Security.Cryptography;
using Sequence.Extensions;
using Sequence.Utils;

namespace Sequence.WaaS
{
    public static class Encryptor
    {
        public static byte[] AES256CBCEncryption(byte[] key, string payload)
        {
            byte[] iv = Org.BouncyCastle.Security.SecureRandom.GetInstance("SHA256PRNG").GenerateSeed(16);
            using AesManaged aesManaged = new AesManaged();
            aesManaged.Key = key;
            aesManaged.IV = iv;
            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, aesManaged.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] payloadBytes = payload.ToByteArray();
            cryptoStream.Write(payloadBytes, 0, payloadBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] result = ByteArrayExtensions.ConcatenateByteArrays(iv, memoryStream.ToArray());
            return result;
        }
    }
}