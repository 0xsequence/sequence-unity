using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Util;
using Newtonsoft.Json;
using Sequence.ABI;
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
        private readonly bool _enableStorage;

        public WalletState(Address address)
        {
            Address = address;
        }

        public async Task Update(Chain chain)
        {
            IsDeployed = await CheckDeployed(chain, Address);
            
            var implementation = await GetImplementation(chain);

            string imageHash;
            if (IsDeployed && implementation.Equals(ExtensionsFactory.Current.Stage2))
            {
                imageHash = await GetOnChainImageHash(chain);
                Debug.Log($"onchain image hash {imageHash}");
            }
            else
            {
                var deployResponse = await GetDeployHash(Address);
                
                DeployHash = deployResponse.deployHash;
                DeployContext = deployResponse.context;
                
                imageHash = DeployHash;
            }
            
            var configUpdates = await _keyMachine.GetConfigUpdates(Address, imageHash);
            if (configUpdates.updates.Length > 0)
                imageHash = configUpdates.updates[^1].toImageHash;

            var config = await GetConfig(imageHash);

            var signerAddress = ExtensionsFactory.Current.Sessions;
            var signerLeaf = config.topology.FindSignerLeaf(signerAddress) as SapientSignerLeaf;
            if (signerLeaf == null)
                throw new Exception($"Could not find signer {signerAddress}");
            
            SessionsImageHash = signerLeaf.imageHash;
            
            Debug.Log($"Config image hash {imageHash}");
            Debug.Log($"SessionsImageHash {SessionsImageHash}");
            Debug.Log($"{JsonConvert.SerializeObject(config.topology.Parse())}");
            
            var sessionsTopology = await GetSessionsTopology(SessionsImageHash);
            Debug.Log($"sessionsTopology {sessionsTopology.JsonSerialize()}");
            
            ConfigUpdates = configUpdates.updates;
            ImageHash = imageHash;
            Config = config;
            SessionsTopology = sessionsTopology;

            await UpdateNonce(chain, 0);
        }

        private async Task<DeployHashReturn> GetDeployHash(Address address)
        {
            var storageKey = $"sequence-deploy-hash-{address}";
            var cached = RetrieveString(storageKey);
            
            if (!string.IsNullOrEmpty(cached))
                return JsonConvert.DeserializeObject<DeployHashReturn>(cached);
                
            var deployResponse = await _keyMachine.GetDeployHash(address);
            _secureStorage.StoreString(storageKey, JsonConvert.SerializeObject(deployResponse));
            
            return deployResponse;
        }

        private async Task<Primitives.Config> GetConfig(string imageHash)
        {
            Debug.Log($"Get config for image hash {imageHash}");
            var storageKey = $"sequence-config-tree-{imageHash}";
            var cached = RetrieveString(storageKey);
            
            if (!string.IsNullOrEmpty(cached))
                return ConfigFromServiceTree(JsonConvert.DeserializeObject<ConfigReturn>(cached));
            
            var response = await _keyMachine.GetConfiguration(imageHash);
            _secureStorage.StoreString(storageKey, JsonConvert.SerializeObject(response));
            
            return ConfigFromServiceTree(response);
        }

        private Primitives.Config ConfigFromServiceTree(ConfigReturn configReturn)
        {
            var tree = configReturn.config.tree.ToString();
            var topology = Topology.FromServiceConfigTree(tree);
            
            return new Primitives.Config
            {
                threshold = new BigInteger(configReturn.config.threshold),
                checkpoint = BigInteger.Parse(configReturn.config.checkpoint),
                topology = topology,
            };
        }

        private async Task<SessionsTopology> GetSessionsTopology(string imageHash)
        {
            var storageKey = $"sequence-sessions-tree-{imageHash}";
            var cached = RetrieveString(storageKey);
            
            if (!string.IsNullOrEmpty(cached))
                return SessionsTopology.FromTree(cached);
            
            var treeReturn = await _keyMachine.GetTree(imageHash);
            var tree = treeReturn.tree.ToString();
            var sessionsTopology = SessionsTopology.FromTree(tree);
            _secureStorage.StoreString(storageKey, tree);
            
            return sessionsTopology;
        }

        private async Task<Address> GetImplementation(Chain chain)
        {
            var response = await new SequenceEthClient(chain).CallContract(new object[] {
                new CallContractData
                {
                    to = Address,
                    data = ABI.ABI.Pack("getImplementation()")
                }
            });

            var data = response.HexStringToByteArray();
            var addressData = data.Slice(data.Length - 20);
            
            var addressStr = addressData.ByteArrayToHexStringWithPrefix();
            return addressStr.IsAddress() ? new Address(addressStr) : null;
        }
        
        private async Task<string> GetOnChainImageHash(Chain chain)
        {
            return await new SequenceEthClient(chain).CallContract(new object[] {
                new CallContractData
                {
                    to = Address,
                    data = ABI.ABI.Pack("imageHash()")
                }
            });
        }

        private async Task<bool> CheckDeployed(Chain chain, Address address)
        {
            var storageKey = $"sequence-deployed-{chain.ToString()}-{address}";
            var cached = RetrieveString(storageKey);
            if (!string.IsNullOrEmpty(cached))
                return true;
            
            var ethClient = new SequenceEthClient(chain);
            var response = await ethClient.CodeAt(address, "pending");
            var isDeployed = response != "0x";
            
            if (isDeployed)
                _secureStorage.StoreString(storageKey, "true");
            
            return isDeployed;
        }

        private async Task UpdateNonce(Chain chain, BigInteger space)
        {
            var response = await new SequenceEthClient(chain).CallContract(new object[] {
                new CallContractData
                {
                    to = Address,
                    data = ABI.ABI.Pack("readNonce(uint256)", space)
                }
            });
            
            Nonce = response == "0x" ? 0 : response.HexStringToBigInteger();
        }

        private string RetrieveString(string key)
        {
            return _enableStorage ? _secureStorage.RetrieveString(key) : null;
        }
    }
}