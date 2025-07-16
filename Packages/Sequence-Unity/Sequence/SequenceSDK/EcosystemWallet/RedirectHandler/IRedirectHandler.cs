using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.Browser
{
    public interface IRedirectHandler
    {
        Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload);
    }
}