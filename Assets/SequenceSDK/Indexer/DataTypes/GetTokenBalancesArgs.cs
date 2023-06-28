namespace Sequence
{
    [System.Serializable]
    public class GetTokenBalancesArgs
    {
        public string accountAddress;
        public string contractAddress;
        public bool includeMetadata;
        public Page page = new Page();

        public GetTokenBalancesArgs(string accountAddress)
        {
            this.accountAddress = accountAddress;
        }

        public GetTokenBalancesArgs(string accountAddress, string contractAddress)
        {
            this.accountAddress = accountAddress;
            this.contractAddress = contractAddress;
        }
        public GetTokenBalancesArgs(string accountAddress, bool includeMetadata)
        {
            this.accountAddress = accountAddress;
            this.includeMetadata = includeMetadata;
        }

        public GetTokenBalancesArgs(string accountAddress, string contractAddress, bool includeMetadata)
        {
            this.accountAddress = accountAddress;
            this.contractAddress = contractAddress;
            this.includeMetadata = includeMetadata;
        }
        public GetTokenBalancesArgs(string accountAddress, string contractAddress, Page page)
        {
            this.accountAddress = accountAddress;
            this.contractAddress = contractAddress;
            this.page = page;
        }

        public GetTokenBalancesArgs(string accountAddress, bool includeMetadata, Page page)
        {
            this.accountAddress = accountAddress;
            this.includeMetadata = includeMetadata;
            this.page = page;
        }

        public GetTokenBalancesArgs(string accountAddress, string contractAddress, bool includeMetadata, Page page)
        {
            this.accountAddress = accountAddress;
            this.contractAddress = contractAddress;
            this.includeMetadata = includeMetadata;
            this.page = page;
        }
    }
}