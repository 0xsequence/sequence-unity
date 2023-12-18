namespace Sequence.WaaS
{
    [System.Serializable]
    public class WalletsArgs
    {
        public Page page;

        public WalletsArgs(Page page = null)
        {
            this.page = page;
        }
    }
}