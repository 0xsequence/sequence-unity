namespace Sequence
{
    [System.Serializable]
    public class PingReturn
    {
        public bool status;

        public PingReturn(bool status)
        {
            this.status = status;
        }
    }
}