using System;
using System.Linq;
using System.Numerics;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Preserve]
    public class RSY
    {
        public BigInt r;
        public BigInt s;
        public int yParity;

        public byte[] Pack()
        {
            if (yParity != 0 && yParity != 1 && yParity != 27 && yParity != 28)
                throw new ArgumentException("yParity must be 0, 1, 27, or 28.");

            var rBytes = r.Value.ByteArrayFromNumber(r.Value.MinBytesFor()).PadLeft(32);
            var sBytes = s.Value.ByteArrayFromNumber(s.Value.MinBytesFor()).PadLeft(32);
            
            if (yParity % 2 == 1)
                sBytes[0] |= 0x80;
            
            return ByteArrayExtensions.ConcatenateByteArrays(rBytes, sBytes);
        }

        public override string ToString()
        {
            return $"R: {r}, S: {s}, Y: {yParity}";
        }

        public static RSY FromString(string input)
        {
            var parts = input.Split(':');
            return new RSY
            {
                r = parts[0].HexStringToBigInteger(),
                s = parts[1].HexStringToBigInteger(),
                yParity = VToYParity(BigInteger.Parse(parts[2]))
            };
        }

        public static RSY Unpack(byte[] rsy)
        {
            if (rsy.Length != 64)
                throw new ArgumentException($"RSY must be exactly 64 bytes ({rsy.Length})");

            var r = rsy[..32].ToBigInteger();

            var yParityAndS = rsy[32..64];
            int yParity = (yParityAndS[0] & 0x80) != 0 ? 1 : 0;

            var sBytes = (byte[])yParityAndS.Clone();
            sBytes[0] &= 0x7F;

            var s = sBytes.ToBigInteger();

            return new RSY
            {
                r = r,
                s = s,
                yParity = yParity,
            };
        }

        public static RSY UnpackFrom65(byte[] sig65)
        {
            var r = sig65.AsSpan(0, 32).ToArray();
            var s = sig65.AsSpan(32, 32).ToArray();
            var v = sig65[64];
            
            return new RSY
            {
                r = r.ToBigInteger(),
                s = s.ToBigInteger(),
                yParity = VToYParity(v)
            };
        }

        public static int VToYParity(BigInteger v)
        {
            if (v == 27) 
                return 0;
            
            if (v == 28) 
                return 1;
            
            return (int)(v % 2);
        }
    }
}