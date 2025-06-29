using System;
using System.Linq;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class RSY : SignatureOfSignerLeaf
    {
        public BigInteger r;
        public BigInteger s;
        public BigInteger yParity;

        public byte[] Pack()
        {
            if (yParity != 0 && yParity != 1 && yParity != 27 && yParity != 28)
                throw new ArgumentException("yParity must be 0, 1, 27, or 28.");

            var rBytes = r.ByteArrayFromNumber(r.MinBytesFor()).PadLeft(32);
            var sBytes = s.ByteArrayFromNumber(s.MinBytesFor()).PadLeft(32);
            
            if (yParity % 2 == 1)
                sBytes[0] |= 0x80;
            
            return ByteArrayExtensions.ConcatenateByteArrays(rBytes, sBytes);
        }

        public static (BigInteger R, BigInteger S, int YParity) Unpack(byte[] rsy)
        {
            if (rsy.Length != 64)
                throw new ArgumentException("RSY must be exactly 64 bytes");

            var r = rsy[..32].ToBigInteger();

            var yParityAndS = rsy[32..64];
            int yParity = (yParityAndS[0] & 0x80) != 0 ? 1 : 0;

            var sBytes = (byte[])yParityAndS.Clone();
            sBytes[0] &= 0x7F;

            var s = sBytes.ToBigInteger();

            return (r, s, yParity);
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