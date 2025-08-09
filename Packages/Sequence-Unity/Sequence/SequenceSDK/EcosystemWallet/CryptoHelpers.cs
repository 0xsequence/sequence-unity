using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using NBitcoin.Secp256k1;
using Nethereum.Util;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public static class EthCrypto
    {
        public static string HashFunctionSelector(string function)
        {
            var sha3 = new Sha3Keccack();
            var hash = sha3.CalculateHash(function);
            return "0x" + hash.Substring(0, 8);
        }
        
        /// <summary>
        /// Recover an ECPubKey from (r,s,yParity) and a 32-byte payload hash.
        /// - r, s: 32-byte big-endian
        /// - yParity: 0 or 1 (normalize 27/28 or EIP-155 before calling)
        /// - payloadHash32: the exact 32-byte digest that was signed
        /// </summary>
        public static ECPubKey RecoverPublicKey(byte[] r, byte[] s, int v, byte[] msg32)
        {
            if (r?.Length != 32 || s?.Length != 32) throw new ArgumentException("r and s must be 32 bytes.");
            if (msg32?.Length != 32) throw new ArgumentException("msg32 must be 32 bytes.");

            // Normalize v → recId 0/1
            int recId = v >= 35 ? ((v - 35) % 2) : (v >= 27 ? v - 27 : v);
            if (recId is < 0 or > 1) throw new ArgumentException("Invalid recovery id (v).");

            var compact = r.Concat(s).ToArray(); // r||s

            if (!SecpRecoverableECDSASignature.TryCreateFromCompact(compact, recId, out var recSig))
                throw new InvalidOperationException("Failed to create recoverable signature.");

            var ctx = Context.Instance;

            if (!ECPubKey.TryRecover(ctx, recSig, msg32, out var pubKey))
                throw new InvalidOperationException("Public key recovery failed.");

            return pubKey;
        }
            
        public static ECPubKey RecoverPublicKey(byte[] sig, byte[] payloadHash32)
        {
            var rsy = RSY.Unpack(sig);
            var r = rsy.r.Value.ByteArrayFromNumber();
            var s = rsy.s.Value.ByteArrayFromNumber();
            var v = rsy.yParity + 27;
            
            return RecoverPublicKey(r, s, v, payloadHash32);
        }

        /// <summary>
        /// If you want X||Y (64 bytes) like many JS libs return.
        /// </summary>
        public static byte[] PublicKeyXY(ECPubKey pubKey)
        {
            var uncompressed = pubKey.ToBytes(false); // 0x04||X||Y
            return uncompressed.Skip(1).ToArray(); // drop 0x04 → 64 bytes
        }

        // Compute EIP-55 address from 64-byte pubkey (X||Y)
        public static string AddressFromPublicKey(byte[] pubkeyXY64)
        {
            if (pubkeyXY64 == null || pubkeyXY64.Length != 64) throw new ArgumentException("pubkey must be 64 bytes");

            // keccak256 of pubkey (no 0x04), take last 20 bytes
            var keccak = Keccak256(pubkeyXY64);
            var addrBytes = keccak.Skip(12).ToArray(); // last 20 bytes
            var hexLower = "0x" + BytesToHex(addrBytes).ToLowerInvariant();
            return ToEip55Checksum(hexLower);
        }

        private static byte[] Keccak256(byte[] data)
        {
            var d = new KeccakDigest(256);
            d.BlockUpdate(data, 0, data.Length);
            var output = new byte[32];
            d.DoFinal(output, 0);
            return output;
        }

        private static string BytesToHex(byte[] bytes) =>
            string.Concat(bytes.Select(b => b.ToString("x2")));

        // EIP-55 checksum casing
        private static string ToEip55Checksum(string hexWith0xLower)
        {
            var hex = hexWith0xLower.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? hexWith0xLower.Substring(2)
                : hexWith0xLower;

            var hash = Keccak256(Encoding.ASCII.GetBytes(hex)).Select(b => b.ToString("x2")).Aggregate(string.Concat);
            var sb = new StringBuilder("0x", 42);
            for (int i = 0; i < hex.Length; i++)
            {
                char c = hex[i];
                int nibble = int.Parse(hash[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                sb.Append((nibble >= 8) ? char.ToUpperInvariant(c) : c);
            }
            return sb.ToString();
        }
    }
}