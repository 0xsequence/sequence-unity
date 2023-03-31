
namespace SequenceSharp
{
    [System.Serializable]
    public class ProviderConfig
    {
        /// <summary>
        /// Sequence Wallet Webapp URL
        /// </summary>
        public string walletAppURL = "https://sequence.app/";

        /// <summary>
        /// A string like "polygon" or "137".
        /// 
        /// The primary network of a dapp and
        /// the default network a
        /// provider will communicate to.
        /// </summary>
        public string defaultNetworkId = "polygon";

#nullable enable
        /// <summary>
        /// A configuration list of networks used by the wallet. This list
        /// is combined with the network list supplied from the wallet upon login,
        /// and settings here take precedence such as overriding a relayer setting, or rpcUrl.
        /// </summary>
        public NetworkConfig[]? networks;

        /// <summary>
        /// The provider rpcUrl of the default network
        /// </summary>
        public string? networkRpcUrl;
#nullable disable
    }
}