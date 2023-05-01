namespace Sequence
{
    [System.Serializable]
    public class GetBalanceUpdatesArgs
    {
        public string contractAddress;
        public int lastUpdateID;
        public Page page;

        public GetBalanceUpdatesArgs()
        {

        }

        public GetBalanceUpdatesArgs(string contractAddress)
        {
            this.contractAddress = contractAddress;
        }

        public GetBalanceUpdatesArgs(string contractAddress, int lastUpdateID)
        {
            this.contractAddress = contractAddress;
            this.lastUpdateID = lastUpdateID;
        }

        public GetBalanceUpdatesArgs(string contractAddress, int lastUpdateID, Page page)
        {
            this.contractAddress = contractAddress;
            this.lastUpdateID = lastUpdateID;
            this.page = page;
        }
    }
}