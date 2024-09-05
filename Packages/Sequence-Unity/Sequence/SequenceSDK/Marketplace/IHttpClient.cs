using System.Threading.Tasks;

namespace Sequence.Marketplace
{
    public interface IHttpClient
    {
        public Task<ReturnType> SendRequest<ArgType, ReturnType>(Chain chain, string endpoint, ArgType args);
    }
}