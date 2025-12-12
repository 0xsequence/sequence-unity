using System.Threading.Tasks;

namespace Sequence.Relayer
{
    public interface IRelayer
    {
        Task<string> Relay(Address to, string data, string quote = null, IntentPrecondition[] preconditions = null);
        Task<FeeOptionsReturn> GetFeeOptions(FeeOptionsArgs args);
        Task<GetMetaTxnReceiptReturn> GetMetaTxnReceipt(string metaTxnID);
        Task<SendMetaTxnReturn> SendMetaTxn(SendMetaTxnArgs args);
    }
}