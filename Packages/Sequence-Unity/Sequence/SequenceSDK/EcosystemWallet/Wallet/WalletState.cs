using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.KeyMachine.Models;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Provider;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using UnityEngine;
using ConfigUpdate = Sequence.EcosystemWallet.KeyMachine.Models.ConfigUpdate;

namespace Sequence.EcosystemWallet
{
    internal class WalletState
    {
        public Address Address { get; }
        public string ImageHash { get; private set; }
        public string SessionsImageHash { get; private set; }
        public bool IsDeployed { get; private set; }
        public string DeployHash { get; private set; }
        public DeployHashContext DeployContext { get; private set; }
        public BigInteger Nonce { get; private set; }
        public Primitives.Config Config { get; private set; }
        public SessionsTopology SessionsTopology { get; private set; }
        public ConfigUpdate[] ConfigUpdates { get; private set; }
        
        private readonly KeyMachineApi _keyMachine = new();
        private readonly ISecureStorage _secureStorage = SecureStorageFactory.CreateSecureStorage();

        public WalletState(Address address)
        {
            Address = address;
        }

        public async Task Update(Chain chain)
        {
            var deployResponse = await GetDeployHash(Address);
            DeployHash = deployResponse.deployHash;
            DeployContext = deployResponse.context;
            
            var configUpdates = await _keyMachine.GetConfigUpdates(Address, DeployHash);
            var imageHash = configUpdates.updates.Length > 0 ? configUpdates.updates[^1].toImageHash : DeployHash;
            
            var config = await GetConfig(imageHash);
            
            var signerLeaf = config.topology.FindSignerLeaf(new Address("0x06aa3a8F781F2be39b888Ac8a639c754aEe9dA29")) as SapientSignerLeaf;
            SessionsImageHash = signerLeaf.imageHash;
            
            var sessionsTopology = await GetSessionsTopology(SessionsImageHash);
            
            ConfigUpdates = configUpdates.updates;
            ImageHash = imageHash;
            Config = config;
            SessionsTopology = sessionsTopology;
            
            await UpdateNonce(chain, 0);

            IsDeployed = await CheckDeployed(chain, Address);
        }

        private async Task<DeployHashReturn> GetDeployHash(Address address)
        {
            var storageKey = $"sequence-deploy-hash-{address}";
            var cached = _secureStorage.RetrieveString(storageKey);
            if (!string.IsNullOrEmpty(cached))
                return JsonConvert.DeserializeObject<DeployHashReturn>(cached);
                
            var deployResponse = await _keyMachine.GetDeployHash(address);
            _secureStorage.StoreString(storageKey, JsonConvert.SerializeObject(deployResponse));
            
            return deployResponse;
        }

        private async Task<Primitives.Config> GetConfig(string imageHash)
        {
            var storageKey = $"sequence-config-{imageHash}";
            var cached = _secureStorage.RetrieveString(storageKey);
            if (!string.IsNullOrEmpty(cached))
                return Primitives.Config.FromJson(cached);
            
            var config = await _keyMachine.GetConfiguration(imageHash);
            _secureStorage.StoreString(storageKey, config.ToJson());
            
            return config;
        }

        private async Task<SessionsTopology> GetSessionsTopology(string imageHash)
        {
            var storageKey = $"sequence-sessions-tree-{imageHash}";
            var cached = _secureStorage.RetrieveString(storageKey);
            if (!string.IsNullOrEmpty(cached))
                return SessionsTopology.FromTree(cached);
            
            var treeReturn = await _keyMachine.GetTree(SessionsImageHash);
            var tree = treeReturn.tree.ToString();
            var sessionsTopology = SessionsTopology.FromTree(tree);
            _secureStorage.StoreString(storageKey, tree);
            
            return sessionsTopology;
        }

        private async Task<bool> CheckDeployed(Chain chain, Address address)
        {
            var storageKey = $"sequence-deployed-{chain.ToString()}-{address}";
            var cached = _secureStorage.RetrieveString(storageKey);
            if (!string.IsNullOrEmpty(cached))
                return false;
            
            var ethClient = new SequenceEthClient(chain);
            var response = await ethClient.CodeAt(address, "pending");
            var isDeployed = response != "0x";
            
            if (isDeployed)
                _secureStorage.StoreString(storageKey, "true");
            
            return isDeployed;
        }

        public async Task UpdateNonce(Chain chain, BigInteger space)
        {
            var function = new FunctionABI("readNonce", false)
            {
                InputParameters = new[]
                {
                    new Parameter("uint256", "_space")
                }
            };

            var encoder = new FunctionCallEncoder();
            var data = encoder.EncodeRequest(function.Sha3Signature, function.InputParameters, space);
            
            var response = await new SequenceEthClient(chain).CallContract(new object[] {
                new
                {
                    to = Address,
                    data
                }
            });
            
            Nonce = response == "0x" ? 0 : response.HexStringToBigInteger();
        }
    }
}