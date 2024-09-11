using System.Security.Cryptography;

namespace Sequence.Utils
{
    public static class SHA256Utils
    {
        public static string SHA256Hash(this string value)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] hash = sha256.ComputeHash(bytes);
            return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        
        public static byte[] SHA256Hash(this byte[] value)
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(value);
        }
    }
}