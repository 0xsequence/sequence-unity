using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.EcosystemWallet.Utils;
using Sequence.Relayer;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.EcosystemWallet
{
    public class SequenceWallet : IWallet
    {
        public Address Address { get; }
        
        private SessionSigner[] _sessionSigners;
        private WalletState _state;
        
        internal SequenceWallet(SessionSigner[] sessionSigners)
        {
            Address = sessionSigners[0].ParentAddress;
            
            _sessionSigners = sessionSigners;
            _state = new WalletState(Address);
        }

        public static IWallet RecoverFromStorage()
        {
            var credentials = SessionStorage.GetSessions();
            var sessionWallets = new SessionSigner[credentials.Length];
            for (var i = 0; i < credentials.Length; i++)
                sessionWallets[i] = new SessionSigner(credentials[i]);

            return new SequenceWallet(sessionWallets);
        }

        public Address[] GetAllSigners()
        {
            return _sessionSigners.Select(x => x.Address).ToArray();
        }
        
        public async Task AddSession(Chain chain, SessionPermissions permissions)
        {
            Assert.IsNotNull(permissions);
            
            var ecosystem = _sessionSigners[0].Ecosystem;
            var client = new EcosystemClient(ecosystem, chain);
            
            var sessionSigner = await client.CreateNewSession(true, permissions, string.Empty);
            _sessionSigners.AddToArray(sessionSigner);
        }
        
        public void Disconnect()
        {
            SessionStorage.Clear();
            _sessionSigners = Array.Empty<SessionSigner>();
        }
        
        public async Task<SignMessageResponse> SignMessage(string message)
        {
            var args = new SignMessageArgs 
            { 
                address = Address, 
                chainId = new BigInt((int)_sessionSigners[0].Chain), 
                message = message
            };

            var url = $"{EcosystemBindings.GetUrl(EcosystemType.Sequence)}/request/sign";

            var handler = RedirectFactory.CreateHandler();
            handler.SetRedirectUrl(RedirectOrigin.GetOriginString());
            
            var response = await handler.WaitForResponse<SignMessageArgs, SignMessageResponse>(url, "signMessage", args);
            
            if (!response.Result)
                throw new Exception("Failed to sign message");
            
            return response.Data;
        }

        public async Task<FeeOption[]> GetFeeOption(Chain chain, Call[] calls)
        {
            await _state.Update(chain);
            
            var transactionData = await SignCalls(chain, calls, false);
            var relayer = new SequenceRelayer(chain);

            var args = new FeeOptionsArgs(transactionData.To, transactionData.To, transactionData.Data);
            var response = await relayer.GetFeeOptions(args);

            return response.options;
        }

        public async Task<string> SendTransaction(Chain chain, Call[] calls, FeeOption feeOption = null)
        {
            await _state.Update(chain);

            if (feeOption != null)
            {
                var encodedFeeOptionData = ABI.ABI.Pack("transfer(address,uint256)",
                    feeOption.to, BigInteger.Parse(feeOption.value)).HexStringToByteArray();
                
                var feeOptionCall = new Call(
                    feeOption.token.contractAddress, 
                    0, 
                    encodedFeeOptionData, 
                    feeOption.gasLimit, 
                    false, 
                    false,
                    BehaviourOnError.revert);

                calls = calls.Unshift(feeOptionCall);
            }
            
            var transactionData = await SignCalls(chain, calls, true);
            
            var relayer = new SequenceRelayer(chain);
            var hash = await relayer.Relay(transactionData.To, transactionData.Data);

            MetaTxnReceipt receipt = null;
            var status = OperationStatus.Pending;
            
            while (status != OperationStatus.Confirmed && status != OperationStatus.Failed)
            {
                var receiptResponse = await relayer.GetMetaTxnReceipt(hash);
                receipt = receiptResponse.receipt;
                status = receipt?.EvaluateStatus() ?? OperationStatus.Unknown;
            }
            
            if (receipt == null)
                throw new Exception("receipt is null");

            return receipt.txnReceipt;
        }
        
        private async Task<TransactionData> SignCalls(Chain chain, Call[] calls, bool checkDeployed)
        {
            var preparedIncrement = PrepareIncrement(null, 0, null);
            if (preparedIncrement != null)
                calls.AddToArray(preparedIncrement);

            var envelope = PrepareTransaction(chain, calls);
            
            var signature = await SignSapient(chain, envelope);
            var sapientSignature = new SapientSignature
            {
                imageHash = _state.SessionsImageHash,
                signature = signature
            };

            var signedEnvelope = envelope.ToSigned(sapientSignature);
            var rawSignature = SignatureHandler.EncodeSignature(signedEnvelope);
            
            rawSignature.suffix = _state.ConfigUpdates
                .Select(u => RawSignature.Decode(u.signature.HexStringToByteArray())).ToArray();

            var callsData = ABI.ABI.Pack("execute(bytes,bytes)",
                envelope.payload.Encode(),
                rawSignature.Encode());
            
            if (!checkDeployed || _state.IsDeployed)
            {
                return new TransactionData
                {
                    To = Address,
                    Data = callsData
                };
            }

            var deployTransaction = BuildDeployTransaction();
            return new TransactionData
            {
                To = new Address("0xf3c7175460BeD3340A1c4dc700fD6C8Cd3F56250"),
                Data = new Calls(0, 0, new Call[]
                {
                    new (deployTransaction.To, 0, deployTransaction.Data.HexStringToByteArray()),
                    new (Address, 0, callsData.HexStringToByteArray())
                }).Encode().ByteArrayToHexStringWithPrefix()
            };
        }

        private TransactionData BuildDeployTransaction()
        {
            var deployTransaction = Erc6492Helper.Deploy(_state.DeployHash, new Erc6492Helper.Context
            {
                creationCode = _state.DeployContext.walletCreationCode,
                factory = _state.DeployContext.factory,
                stage1 = _state.DeployContext.mainModule,
                stage2 = _state.DeployContext.mainModuleUpgradable
            });

            return new TransactionData
            {
                To = new Address(deployTransaction.To),
                Data = deployTransaction.Data
            };
        }

        private Call PrepareIncrement(Address wallet, BigInteger chainId, Calls calls)
        {
            return null;
        }

        private Envelope<Calls> PrepareTransaction(Chain chain, Call[] calls)
        {
            return new Envelope<Calls>
            {
                chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]),
                wallet = Address,
                configuration = _state.Config,
                payload = new Calls(0, _state.Nonce, calls)
            };
        }

        private async Task<SignatureOfSapientSignerLeaf> SignSapient(Chain chain, Envelope<Calls> envelope)
        {
            var calls = envelope.payload.calls;
            if (calls.Length == 0)
                throw new Exception("calls is empty");

            var implicitSigners = new List<Address>();
            var explicitSigners = new List<Address>();

            var signers = await FindSignersForCalls(chain, calls);
            
            var signatures = new SessionCallSignature[signers.Length];
            for (var i = 0; i < signers.Length; i++)
            {
                var signature = signers[i].SignCall(calls[i], _state.SessionsTopology, envelope.payload.space, envelope.payload.nonce);
                signatures[i] = signature;
            }

            foreach (var signer in signers)
            {
                if (signer.IsExplicit)
                    explicitSigners.Add(signer.Address);
                else
                    implicitSigners.Add(signer.Address);
            }
            
            var sessionSignatures = SessionCallSignature.EncodeSignatures(
                signatures,
                _state.SessionsTopology, 
                explicitSigners.ToArray(), 
                implicitSigners.ToArray());
            
            return new SignatureOfSapientSignerLeaf
            {
                curType = SignatureOfSapientSignerLeaf.Type.sapient,
                address = _state.Sessions,
                data = sessionSignatures
            };
        }
        
        private async Task<SessionSigner[]> FindSignersForCalls(Chain chain, Call[] calls)
        {
            var identitySigner = _state.SessionsTopology.GetIdentitySigner();
            if (identitySigner == null)
                throw new Exception("identitySigner is null");

            var blacklist = _state.SessionsTopology.GetImplicitBlacklist();
            if (blacklist == null)
                throw new Exception("blacklist is null");
            
            var validImplicitSigners = _sessionSigners.Where(s => 
                !s.IsExplicit &&
                s.IdentitySigner.Equals(identitySigner) &&
                !blacklist.Contains(s.Address)
                ).ToArray();

            var explicitSigners = _state.SessionsTopology.GetExplicitSigners();
            var validExplicitSigners = _sessionSigners
                .Where(s => s.IsExplicit && 
                            Array.Exists(explicitSigners, es => es.Equals(s.Address))).ToArray();

            var availableSigners = ArrayUtils.CombineArrays(validImplicitSigners, validExplicitSigners);
            if (availableSigners.Length == 0)
                throw new Exception("no valid signers found");
            
            var signers = new List<SessionSigner>();
            foreach (var call in calls)
            {
                foreach (var signer in availableSigners)
                {
                    Debug.Log($"Checking signer {signer.Address}");
                    var supported = await signer.IsSupportedCall(call, chain, _state.SessionsTopology);
                    if (supported)
                    {
                        signers.Add(signer);
                        Debug.Log($"Using signer {signer.Address}");
                        break;
                    }
                }
            }
            
            return signers.ToArray();
        }
    }
}