namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class DelayedEncode : SequenceSDK.WaaS.Transaction
    {
        public DelayedEncodeData data { get; private set; }
        public string type { get; private set; } = "delayedEncode";
        public string to { get; private set; }
        public string value { get; private set; }
        
        public DelayedEncode(string to, string value, DelayedEncodeData data)
        {
            this.to = to;
            this.value = value;
            this.data = data;
        }
    }
}