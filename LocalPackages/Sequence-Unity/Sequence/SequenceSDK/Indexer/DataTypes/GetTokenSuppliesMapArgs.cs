namespace Sequence
{
    using System.Collections.Generic;

    [System.Serializable]
    public class GetTokenSuppliesMapArgs
    {
        /// <summary>
        /// Key: ContractAddress, Value: tokenId's
        /// </summary>
        public Dictionary<string, string[]> tokenMap;
        public bool includeMetadata;

        public GetTokenSuppliesMapArgs()
        {

        }

        public GetTokenSuppliesMapArgs(Dictionary<string, string[]> tokenMap)
        {
            this.tokenMap = tokenMap;
        }

        public GetTokenSuppliesMapArgs(Dictionary<string, string[]> tokenMap, bool includeMetadata)
        {
            this.tokenMap = tokenMap;
            this.includeMetadata = includeMetadata;
        }
    }
}