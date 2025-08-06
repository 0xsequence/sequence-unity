using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.KeyMachine.Models;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    internal class KeyMachineApi
    {
        private const string DefaultHost = "https://v3-keymachine.sequence-dev.app";
        
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
        
        public async Task<Primitives.Config> GetConfiguration(string imageHash)
        {
            var args = new ConfigArgs(imageHash);
            var response = await SendRequest<ConfigArgs, ConfigReturn>("Config", args);
            
            var topology = Topology.FromServiceConfigTree(response.config.tree.ToString());
            return new Primitives.Config
            {
                threshold = new BigInteger(response.config.threshold),
                checkpoint = BigInteger.Parse(response.config.checkpoint),
                topology = topology,
            };
        }

        private async Task<TReturn> SendRequest<TArgs, TReturn>(string endpoint, TArgs args)
        {
            return await _httpClient.SendPostRequest<TArgs, TReturn>("rpc/Sessions/" + endpoint, args);
        }
    }
}