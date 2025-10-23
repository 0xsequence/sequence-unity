using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public class GuardService
    {
        private readonly HttpClient _client;
        
        public GuardService(string url)
        {
            _client = new HttpClient(url);
        }
        
        public async Task<SignWithResponse> SignWith(SignWithArgs args)
        {
            return await SendRequest<SignWithArgs, SignWithResponse>("SignWith", args);
        }

        private async Task<TResponse> SendRequest<TArgs, TResponse>(string endpoint, TArgs args)
        {
            var path = $"rpc/Guard/{endpoint}";
            return await _client.SendPostRequest<TArgs, TResponse>(path, args);
        }
    }
}