using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.Relayer
{
    public interface IRelayer
    {
        Task<string> Relay(Address to, string data, string quote = null, IntentPrecondition[] preconditions = null);
        Task<OperationStatus> Status(string opHash, BigInteger chainId);
        Task<FeeOptionsReturn> GetFeeOptions(FeeOptionsArgs args);
        Task<SendMetaTxnReturn> SendMetaTxn(SendMetaTxnArgs args);
    }
}