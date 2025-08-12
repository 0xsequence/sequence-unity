using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Sequence.EcosystemWallet.KeyMachine.Models;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;
using ConfigUpdate = Sequence.EcosystemWallet.KeyMachine.Models.ConfigUpdate;

namespace Sequence.EcosystemWallet
{
    internal class WalletState
    {
        // Why this signer leaf?
        public Address Sessions = new Address("0x06aa3a8F781F2be39b888Ac8a639c754aEe9dA29");
        
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

        public WalletState(Address address)
        {
            Address = address;
        }

        public async Task Update(Chain chain)
        {
            var deployResponse = await _keyMachine.GetDeployHash(Address);
            DeployHash = deployResponse.deployHash;
            DeployContext = deployResponse.context;
            
            var configUpdates = await _keyMachine.GetConfigUpdates(Address, deployResponse.deployHash);
            var imageHash = configUpdates.updates.Length > 0 ? configUpdates.updates[^1].toImageHash : deployResponse.deployHash;
            
            var config = await _keyMachine.GetConfiguration(imageHash);
            
            Debug.Log($"Config: {config.ToJson()}");
            
            var signerLeaf = config.topology.FindSignerLeaf(Sessions) as SapientSignerLeaf;
            SessionsImageHash = signerLeaf.imageHash;
            
            var treeReturn = await _keyMachine.GetTree(SessionsImageHash);
            var sessionsTopology = SessionsTopology.FromTree(treeReturn.tree.ToString());
            
            ConfigUpdates = configUpdates.updates;
            ImageHash = imageHash;
            Config = config;
            SessionsTopology = sessionsTopology;
            
            Debug.Log($"Sessions Topology {sessionsTopology.JsonSerialize()}");

            await UpdateNonce(chain, 0);

            var ethClient = new SequenceEthClient(chain);
            var response = await ethClient.CodeAt(Address, "pending");
            IsDeployed = response != "0x";
            
            Debug.Log($"Is deployed: {IsDeployed}");
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
            Debug.Log($"Nonce: {Nonce}");
        }
    }
}