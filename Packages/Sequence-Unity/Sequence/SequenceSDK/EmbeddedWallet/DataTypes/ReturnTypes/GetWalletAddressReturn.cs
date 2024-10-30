using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class GetWalletAddressReturn
    {
        public string address;

        public GetWalletAddressReturn(string address)
        {
            this.address = address;
        }
    }
}