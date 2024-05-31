namespace Sequence
{
    [System.Serializable]
    public class GetTokenSuppliesArgs
    {
        public string contractAddress;
        public bool includeMetadata;
        public Page page;

        public GetTokenSuppliesArgs(string contractAddress)
        {
            this.contractAddress = contractAddress;
        }

        public GetTokenSuppliesArgs(string contractAddress, bool includeMetadata)
        {
            this.contractAddress = contractAddress;
            this.includeMetadata = includeMetadata;
        }

        public GetTokenSuppliesArgs(string contractAddress, bool includeMetadata, Page page)
        {
            this.contractAddress = contractAddress;
            this.includeMetadata = includeMetadata;
            this.page = page;
        }
    }
}
