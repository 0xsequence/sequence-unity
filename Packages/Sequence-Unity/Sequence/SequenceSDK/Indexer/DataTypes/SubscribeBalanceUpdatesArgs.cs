namespace Sequence
{
    [System.Serializable]
    public class SubscribeBalanceUpdatesArgs
    {
        public string contractAddress;

        public SubscribeBalanceUpdatesArgs(string contractAddress)
        {
            this.contractAddress = contractAddress;
        }
    }
}