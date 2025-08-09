using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Provider;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    internal class WalletState
    {
        public Address Address { get; }
        public string ImageHash { get; private set; }
        public bool IsDeployed { get; private set; } = true; // At what condition is this false?
        public BigInteger Nonce { get; private set; }
        public Primitives.Config Config { get; private set; }
        public SessionsTopology SessionsTopology { get; private set; }
        
        private readonly KeyMachineApi _keyMachine = new();

        public WalletState(Address address)
        {
            this.Address = address;
        }

        public async Task Update()
        {
            var deployResponse = await _keyMachine.GetDeployHash(Address);
            var config = await _keyMachine.GetConfiguration(deployResponse.deployHash);
            
            Debug.Log($"Config: {config.ToJson()}");

            // Why this signer leaf?
            var signerLeaf = config.topology.FindSignerLeaf(new Address("0x23c2eB9958BcAC9E531E785c4f65e91F1F426142")) as SapientSignerLeaf;
            var treeReturn = await _keyMachine.GetTree(signerLeaf.imageHash);
            var sessionsTopology = SessionsTopology.FromTree(treeReturn.tree.ToString());
            
            ImageHash = deployResponse.deployHash;
            Config = config;
            SessionsTopology = sessionsTopology;
            
            Debug.Log($"Sessions Topology {sessionsTopology.JsonSerialize()}");

            await UpdateNonce(0);
        }

        public async Task UpdateNonce(BigInteger space)
        {
            var function = new FunctionABI("readNonce", false)
            {
                InputParameters = new[]
                {
                    new Parameter("uint256", "_space")
                }
            };

            var encoder = new FunctionCallEncoder();
            var encodedRequest = encoder.EncodeRequest(function.Sha3Signature, function.InputParameters, space);
            
            var response = await new SequenceEthClient(Chain.TestnetArbitrumSepolia).CallContract(new object[] {
                new
                {
                    Address,
                    encodedRequest
                }
            });
            
            Nonce = response == "0x" ? 0 : BigInteger.Parse(response);
        }
    }
}