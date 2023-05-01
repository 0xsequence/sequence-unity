namespace Sequence
{
    [System.Serializable]
    public class GetTokenSuppliesReturn
    {
        public Page page;
        public ContractType contractType;
        public TokenSupply[] tokenIDs;
    }
}