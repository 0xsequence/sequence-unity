using System.Threading.Tasks;

namespace Sequence.Marketplace
{
    public interface IHttpClient
    {
        public Task<ReturnType> SendRequest<ReturnType>(Chain chain, string url);
        public Task<ReturnType> SendRequest<ArgType, ReturnType>(Chain chain, string endpoint, ArgType args);
        public Task<ReturnType> SendRequest<ArgType, ReturnType>(string url, ArgType args);
    }
}