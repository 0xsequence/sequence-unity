using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Relayer;

namespace Sequence.EcosystemWallet
{
    public interface IWallet
    {
        Address Address { get; }
        Task<SignMessageResponse> SignMessage(string message);
        Task<FeeOption[]> GetFeeOption(Call[] calls);
        Task<string> SendTransaction(Call[] calls, FeeOption feeOption = null);
    }
}