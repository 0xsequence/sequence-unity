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
            {
                throw new ArgumentException("yParity must be 0, 1, 27, or 28.");
            }

            byte[] rBytes = r.ByteArrayFromNumber().PadLeft(32);
            byte[] sBytes = s.ByteArrayFromNumber().PadLeft(32);
            byte[] yByte = new byte[] { (byte)(yParity % 2) };

            return rBytes.Concat(sBytes).Concat(yByte).ToArray();
        }
    }
}