namespace Sequence
{
    [System.Serializable]
    public class SubscribeEventsArgs
    {
        public EventFilter filter;

        public SubscribeEventsArgs(EventFilter filter)
        {
            this.filter = filter;
        }
    }
}