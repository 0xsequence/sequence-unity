namespace Sequence.WaaS
{
    [System.Serializable]
    public class GetWalletAddressArgs
    {
        public uint accountIndex;

        public GetWalletAddressArgs(uint accountIndex)
        {
            this.accountIndex = accountIndex;
        }
    }
}