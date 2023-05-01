namespace Sequence
{
    public class WalletSession
    {
#nullable enable
        /// <summary>
        /// Sequence Wallet intrinsic details.
        /// </summary>
        public WalletContext? walletContext;

        /// <summary>
        /// Account address of the wallet.
        /// </summary>
        public string? accountAddress;

        /// <summary>
        /// Networks in use for this session. The default/dApp network will show up as the first one in the list as the "main chain".
        /// </summary>
        public NetworkConfig[]? networks;

        // Caching provider responses for things such as account and chainId
        // providerCache?: { [key: string]: any
    }
#nullable disable
}