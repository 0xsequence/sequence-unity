using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Sequence.ABI;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Utils;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    internal class TransactionService
    {
        private SessionSigner[] _sessionSigners;
        private WalletState _state;
        
        public TransactionService(SessionSigner[] sessionSigners, WalletState state)
        {
            _sessionSigners = sessionSigners;
            _state = state;
        }
        
        public async Task<(Address To, string Data)> SignAndBuild(Chain chain, Call[] calls, bool checkDeployed)
        {
            var preparedIncrement = await PrepareIncrement(null, chain, calls);
            if (preparedIncrement != null)
                calls = calls.Unshift(preparedIncrement);

            var envelope = PrepareTransaction(chain, calls);
            
            var signatureService = new SignatureService(_sessionSigners, _state.SessionsTopology);
            var signature = await signatureService.SignCalls(chain, _state.SessionsImageHash, envelope, _state.ConfigUpdates);

            var callsData = ABI.ABI.Pack("execute(bytes,bytes)",
                envelope.payload.Encode(),
                signature.Encode());
            
            if (!checkDeployed || _state.IsDeployed)
            {
                return (_state.Address, callsData);
            }

            // Not relevant for signing calls for getting fee options
            // If the wallet was not yet deployed onchain, let's make a deploy transaction first

            var deployContext = _state.DeployContext;
            var deployTransaction = Erc6492Helper.Deploy(_state.DeployHash, new Erc6492Helper.Context
            {
                creationCode = deployContext.walletCreationCode,
                factory = deployContext.factory,
                stage1 = deployContext.mainModule,
                stage2 = deployContext.mainModuleUpgradable
            });
            
            return (new Address(deployContext.guestModule), new Calls(0, 0, new Call[]
            {
                new (deployTransaction.To, 0, deployTransaction.Data.HexStringToByteArray()),
                new (_state.Address, 0, callsData.HexStringToByteArray())
            }).Encode().ByteArrayToHexStringWithPrefix());
        }
        
        private async Task<Call> PrepareIncrement(Address wallet, Chain chain, Call[] calls)
        {
            var signers = await new SignerService(_sessionSigners, _state.SessionsTopology).FindSignersForCalls(
                chain, calls);

            var signersToCallsMap = new Dictionary<SessionSigner, List<Call>>();
            for (var i = 0; i < signers.Length; i++)
            {
                var signer = signers[i];
                if (signersToCallsMap.ContainsKey(signer))
                    signersToCallsMap[signer].Add(calls[i]);
                else
                    signersToCallsMap[signer] = new List<Call> { calls[i] };
            }

            var increments = new List<UsageLimit>();
            foreach (var kvPair in signersToCallsMap)
            {
                if (!kvPair.Key.IsExplicit)
                    continue;
                
                var increment = await kvPair.Key.PrepareIncrements(chain, kvPair.Value.ToArray(), _state.SessionsTopology);
                if (increment != null)
                    increments.Add(increment);
            }
            
            if (increments.Count == 0)
                return null;

            var args = new List<Tuple<FixedByte, BigInteger>>();
            for (var i = 0; i < increments.Count; i++)
            {
                args.Add(new Tuple<FixedByte, BigInteger>(
                    new FixedByte(32, increments[i].UsageHash.HexStringToByteArray()), 
                    increments[i].UsageAmount));
            }
            
            var incrementData = ABI.ABI.Pack("incrementUsageLimit((bytes32,uint256)[])", args);
            return new Call(ExtensionsFactory.Current.Sessions, 0, incrementData.HexStringToByteArray());
        }

        private Envelope<Calls> PrepareTransaction(Chain chain, Call[] calls)
        {
            return new Envelope<Calls>
            {
                chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]),
                wallet = _state.Address,
                configuration = _state.Config,
                payload = new Calls(0, _state.Nonce, calls)
            };
        }
    }
}