namespace Sequence.EmbeddedWallet
{
    public class SequenceContractCall : Transaction
    {
        public const string TypeIdentifier = "contractCall";

        public AbiData data;
        public string to;
        public string type = TypeIdentifier;
        public string value;
        
        public SequenceContractCall(string contractAddress, AbiData data, string value = "0")
        {
            this.to = contractAddress;
            this.value = value;
            this.data = data;
        }
    }
}