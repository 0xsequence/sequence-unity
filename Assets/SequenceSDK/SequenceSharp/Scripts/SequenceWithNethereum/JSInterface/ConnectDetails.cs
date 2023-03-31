namespace SequenceSharp
{
    public class ConnectDetails
    {
#nullable enable
        /// <summary>
        /// ChainID (In Hex) is defined by EIP-1193 expected fields.
        /// </summary>
        public string? chainId;

        /// <summary>
        /// Error (In Hex) is defined by EIP-1193 expected fields.
        /// </summary>
        public string? error;

        /// <summary>
        /// Did user accept the connection request?
        /// </summary>
        public bool connected;

        /// <summary>
        /// Includes account & network information needed by the dApp wallet provider.
        /// </summary>
        public WalletSession? session;

        /// <summary>
        /// A signed typedData (EIP-712) payload using ETHAuth domain.
        /// NOTE: The proof is signed to the `authChainId`, as the canonical auth chain.
        /// </summary>
        public ETHAuthProof? proof;
#nullable disable
    }
}