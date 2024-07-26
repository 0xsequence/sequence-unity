using Sequence.EmbeddedWallet;

namespace Sequence.Relayer
{
    public interface IQueueableTransaction
    {
        public Transaction BuildTransaction();
        public string ToString();
    }
}