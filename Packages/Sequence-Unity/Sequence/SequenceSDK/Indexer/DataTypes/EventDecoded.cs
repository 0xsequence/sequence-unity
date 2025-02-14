namespace Sequence
{
    [System.Serializable]
    public class EventDecoded
    {
        public string topicHash;
        public string eventSig;
        public string[] types;
        public string[] names;
        public string[] values;
    }
}