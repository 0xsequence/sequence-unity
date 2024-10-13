namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class DelayedEncode : Transaction
    {
        public const string TypeIdentifier = "delayedEncode";
        public DelayedEncodeData data { get; private set; }
        public string to { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
        public string value { get; private set; }
        
        public DelayedEncode(string contractAddress, string value, DelayedEncodeData data)
        {
            this.to = contractAddress;
            this.value = value;
            this.data = data;
        }
    }
}