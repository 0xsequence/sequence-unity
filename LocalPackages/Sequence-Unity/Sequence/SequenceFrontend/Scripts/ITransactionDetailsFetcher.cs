using System;
using System.Threading.Tasks;

namespace Sequence.Demo
{
    public interface ITransactionDetailsFetcher
    {
        public event Action<FetchTransactionDetailsResult> OnTransactionDetailsFetchSuccess; 
        public Task FetchTransactions(int maxToFetch);
        public Task FetchTransactionsFromContract(string contractAddress, int maxToFetch);
        public Task FetchTransactionsFromContracts(string[] contractAddresses, int maxToFetch);
        public void Refresh();
    }

    public class FetchTransactionDetailsResult
    {
        public TransactionDetails[] Content;
        public bool MoreToFetch;

        public FetchTransactionDetailsResult(TransactionDetails[] content, bool moreToFetch)
        {
            this.Content = content;
            this.MoreToFetch = moreToFetch;
        }
    }
}