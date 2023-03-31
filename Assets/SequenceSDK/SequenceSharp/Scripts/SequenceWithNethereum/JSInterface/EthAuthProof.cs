namespace SequenceSharp
{
    public class ETHAuthProof
    {
        /// <summary>
        /// EIP712 typed-data payload for ETHAuth domain as input.
        /// </summary>
        public TypedData typedData;
        /// <summary>
        /// Signature encoded in an ETHAuth proof string.
        /// </summary>
        public string proofString;
    }
}