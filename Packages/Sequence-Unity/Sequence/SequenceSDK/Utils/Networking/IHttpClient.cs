using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.Utils
{
    public interface IHttpClient
    {
        Task<TResponse> SendPostRequest<TArgs, TResponse>(string path, TArgs args, Dictionary<string, string> headers = null);
        Task<TResponse> SendGetRequest<TResponse>(string path, Dictionary<string, string> headers = null);
    }
}