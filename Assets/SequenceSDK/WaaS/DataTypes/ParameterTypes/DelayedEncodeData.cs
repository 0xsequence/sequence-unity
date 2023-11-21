namespace SequenceSDK.WaaS
{
    [System.Serializable]
    public class DelayedEncodeData
    {
        public string abi { get; private set; }
        public object[] args { get; private set; } // Todo: figure out how to marshal/unmarshal this field https://docs.sequence.xyz/waas/implementation/payloads#delayedencode
        public string func { get; private set; }
    }
}