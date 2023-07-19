namespace Sequence.WaaS
{
    [System.Serializable]
    public class SignMessageArgs
    {
        public uint chainId;
        public string accountAddress;
        public string message;

        public SignMessageArgs(uint chainId, string accountAddress, string message)
        {
            this.chainId = chainId;
            this.accountAddress = accountAddress;
            this.message = message;
        }
    }
}