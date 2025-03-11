using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class GetWalletAddressReturn
    {
        public string address;

        [Preserve]
        public GetWalletAddressReturn(string address)
        {
            this.address = address;
        }
    }
}