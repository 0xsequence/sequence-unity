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
            return await _httpClient.SendPostRequest<DeployHashArgs, DeployHashReturn>("rpc/Sessions/DeployHash", args);
        }
        
        public async Task<Primitives.Config> GetConfiguration(string imageHash)
        {
            var args = new ConfigArgs(imageHash);
            var response = await _httpClient.SendPostRequest<ConfigArgs, ConfigReturn>("rpc/Sessions/Config", args);
            
            var topology = Topology.FromServiceConfigTree(response.config.tree.ToString());
            return new Primitives.Config
            {
                threshold = new BigInteger(response.config.threshold),
                checkpoint = BigInteger.Parse(response.config.checkpoint),
                topology = topology,
            };
        } 
    }
}