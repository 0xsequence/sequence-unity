namespace Sequence.EmbeddedWallet
{
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