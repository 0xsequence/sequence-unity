namespace Sequence.WaaS
{
    [System.Serializable]
    public class CreateWalletArgs
    {
        public uint accountIndex;

        public CreateWalletArgs(uint accountIndex)
        {
            this.accountIndex = accountIndex;
        }
    }
}