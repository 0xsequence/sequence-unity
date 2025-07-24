namespace Sequence.Relayer
{
    public class FeeOptionsArgs
    {
        public Address wallet;
        public string to;
        public byte[] data;
        public bool simulate;

        public FeeOptionsArgs(Address wallet, string to, byte[] data, bool simulate = false)
        {
            this.wallet = wallet;
            this.to = to;
            this.data = data;
            this.simulate = simulate;
        }
    }
}