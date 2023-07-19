using System;
using Sequence.Core.Signature;

namespace Sequence.Core.V2.Signature
{
    public static class SignatureJoinParameterValidator
    {
        /// <summary>
        /// Throws an exception if otherSignature is not of type T or if the threshold and/or checkpoint don't match between each signature
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signature"></param>
        /// <param name="otherSignature"></param>
        /// <returns></returns>
        public static T ValidateParameters<T>(T signature, ISignature otherSignature) where T : ISignature
        {
            if (!(otherSignature is T other))
            {
                throw new ArgumentException($"{nameof(otherSignature)} Expected {typeof(T)}, got {otherSignature.GetType()}");
            }

            if (signature.Threshold() != other.Threshold())
            {
                throw new ArgumentOutOfRangeException($"Threshold mismatch: {signature.Threshold()} != {other.Threshold()}");
            }

            if (signature.Checkpoint() != other.Checkpoint())
            {
                throw new ArgumentOutOfRangeException($"Checkpoint mismatch: {signature.Checkpoint()} != {other.Checkpoint()}");
            }

            return other;
        }
    }
}
