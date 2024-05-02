namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class DelayedEncode : Sequence.WaaS.Transaction
    {
        public const string TypeIdentifier = "delayedEncode";
        public DelayedEncodeData data;
        public string to;
        public string type = TypeIdentifier;
        public string value;
        
        public DelayedEncode(string to, string value, DelayedEncodeData data)
        {
            this.to = to;
            this.value = value;
            this.data = data;
        }
    }
}