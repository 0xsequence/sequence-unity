using System.Threading.Tasks;
using Sequence.EcosystemWallet.KeyMachine.Models;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    internal class KeyMachineApi
    {
        private const string DefaultHost = "https://keymachine.sequence.app";
        
        private readonly string _host;
        private readonly HttpClient _httpClient;

        public KeyMachineApi(string host = null)
        {
            _host = host ?? DefaultHost;
            _httpClient = new HttpClient(_host);
        }
        
        public async Task<DeployHashReturn> GetDeployHash(Address walletAddress)
        {
            var args = new DeployHashArgs(walletAddress);
            return await SendRequest<DeployHashArgs, DeployHashReturn>("DeployHash", args);
        }

        public async Task<TreeReturn> GetTree(string imageHash)
        {
            var args = new TreeArgs(imageHash);
            return await SendRequest<TreeArgs, TreeReturn>("Tree", args);
        }
        
        public async Task<ConfigUpdatesReturn> GetConfigUpdates(string wallet, string fromImageHash)
        {
            var args = new ConfigUpdatesArgs(wallet, fromImageHash);
            return await SendRequest<ConfigUpdatesArgs, ConfigUpdatesReturn>("ConfigUpdates", args);
        }
        
        public async Task<ConfigReturn> GetConfiguration(string imageHash)
        {
            var args = new ConfigArgs(imageHash);
            return await SendRequest<ConfigArgs, ConfigReturn>("Config", args);
        }

        private async Task<TReturn> SendRequest<TArgs, TReturn>(string endpoint, TArgs args)
        {
            return await _httpClient.SendPostRequest<TArgs, TReturn>("rpc/Sessions/" + endpoint, args);
        }
    }
}