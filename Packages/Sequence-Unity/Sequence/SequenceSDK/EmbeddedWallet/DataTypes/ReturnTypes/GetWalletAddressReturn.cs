using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [System.Serializable]
    public class GetWalletAddressReturn
    {
        public string address;

        [UnityEngine.Scripting.Preserve]
        public GetWalletAddressReturn(string address)
        {
            this.address = address;
        }
    }
}