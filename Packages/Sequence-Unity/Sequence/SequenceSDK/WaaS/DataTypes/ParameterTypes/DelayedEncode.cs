namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class DelayedEncode : Sequence.WaaS.Transaction
    {
        public const string TypeIdentifier = "delayedEncode";
        public DelayedEncodeData data { get; private set; }
        public string to { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
        public string value { get; private set; }
        
        public DelayedEncode(string to, string value, DelayedEncodeData data)
        {
            this.to = to;
            this.value = value;
            this.data = data;
        }
    }
}