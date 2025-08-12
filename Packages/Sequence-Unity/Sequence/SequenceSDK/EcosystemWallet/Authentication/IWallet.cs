using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Relayer;

namespace Sequence.EcosystemWallet
{
    public interface IWallet
    {
        Address Address { get; }
        Task<SignMessageResponse> SignMessage(string message);
        Task<FeeOption[]> GetFeeOption(Chain chain, Call[] calls);
        Task<string> SendTransaction(Chain chain, Call[] calls, FeeOption feeOption = null);
    }
}