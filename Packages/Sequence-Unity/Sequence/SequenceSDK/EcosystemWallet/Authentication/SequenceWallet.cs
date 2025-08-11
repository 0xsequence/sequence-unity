using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Relayer;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public class SequenceWallet : IWallet, IDisposable
    {
        public static Action<SequenceWallet> OnWalletCreated;
        
        public Address Address { get; }
        public SessionSigner[] SessionSigners { get; private set; }

        private WalletState _state;
        
        internal SequenceWallet(SessionSigner[] sessionSigners)
        {
            SessionSigners = sessionSigners;
            Address = sessionSigners[0].ParentAddress;
            OnWalletCreated?.Invoke(this);
            _state = new WalletState(Address);
            
            SequenceConnect.SessionsChanged += SessionsChanged;
        }

        public void Dispose()
        {
            SequenceConnect.SessionsChanged -= SessionsChanged;
        }

        private void SessionsChanged(SessionSigner[] sessionWallets)
        {
            SessionSigners = sessionWallets;
        }
        
        public async Task<SignMessageResponse> SignMessage(string message)
        {
            var args = new SignMessageArgs 
            { 
                address = Address, 
                chainId = new BigInt((int)SessionSigners[0].Chain), 
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

        public async Task<FeeOption[]> GetFeeOption(Call[] calls)
        {
            await _state.Update();
            var transactionData = SignCalls(calls);
            var relayer = new SequenceRelayer(SessionSigners[0].Chain);

            var args = new FeeOptionsArgs(Address, transactionData.To, transactionData.Data);
            var response = await relayer.GetFeeOptions(args);

            return response.options;
        }

        public async Task<string> SendTransaction(Call[] calls, FeeOption feeOption = null)
        {
            await _state.Update();

            if (feeOption != null)
            {
                var encodedFeeOptionData = ABI.ABI.Pack("transfer(address,uint256)",
                    feeOption.to, feeOption.value).HexStringToByteArray();
                
                var feeOptionCall = new Call(
                    feeOption.token, 
                    0, 
                    encodedFeeOptionData, 
                    feeOption.gasLimit, 
                    false, 
                    false,
                    BehaviourOnError.revert);

                calls = calls.Unshift(feeOptionCall);
            }
            
            var transactionData = SignCalls(calls);
            
            var relayer = new SequenceRelayer(SessionSigners[0].Chain);
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
        
        private TransactionData SignCalls(Call[] calls)
        {
            var preparedIncrement = PrepareIncrement(null, 0, null);
            if (preparedIncrement != null)
                calls.AddToArray(preparedIncrement);

            var envelope = PrepareTransaction(calls);
            
            var signature = SignSapient(envelope);
            var sapientSignature = new SapientSignature
            {
                imageHash = _state.SessionsImageHash,
                signature = signature
            };

            var signedEnvelope = envelope.ToSigned(sapientSignature);
            var rawSignature = SignatureHandler.EncodeSignature(signedEnvelope);
            
            rawSignature.suffix = _state.ConfigUpdates
                .Select(u => RawSignature.Decode(u.signature.HexStringToByteArray())).ToArray();

            if (_state.IsDeployed)
            {
                return new TransactionData
                {
                    To = Address,
                    Data = ABI.ABI.Pack("execute(bytes,bytes)", 
                        envelope.payload.Encode(), 
                        rawSignature.Encode())
                };
            }
            
            throw new Exception("wallet status is not deployed");
        }

        private Call PrepareIncrement(Address wallet, BigInteger chainId, Calls calls)
        {
            return null;
        }

        private Envelope<Calls> PrepareTransaction(Call[] calls)
        {
            return new Envelope<Calls>
            {
                chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[SessionSigners[0].Chain]),
                wallet = Address,
                configuration = _state.Config,
                payload = new Calls(0, _state.Nonce, calls)
            };
        }

        private SignatureOfSapientSignerLeaf SignSapient(Envelope<Calls> envelope)
        {
            var calls = envelope.payload.calls;
            if (calls.Length == 0)
                throw new Exception("calls is empty");

            var implicitSigners = new List<Address>();
            var explicitSigners = new List<Address>();

            var signers = FindSignersForCalls(calls);
            
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
        
        private SessionSigner[] FindSignersForCalls(Call[] calls)
        {
            var identitySigner = _state.SessionsTopology.GetIdentitySigner();
            if (identitySigner == null)
                throw new Exception("identitySigner is null");

            var blacklist = _state.SessionsTopology.GetImplicitBlacklist();
            if (blacklist == null)
                throw new Exception("blacklist is null");
            
            var validImplicitSigners = SessionSigners.Where(s => 
                !s.IsExplicit &&
                s.IdentitySigner.Equals(identitySigner) &&
                !blacklist.Contains(s.Address)
                ).ToArray();

            var explicitSigners = _state.SessionsTopology.GetExplicitSigners();
            var validExplicitSigners = SessionSigners
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
                    if (signer.IsSupportedCall(call, _state.SessionsTopology))
                        signers.Add(signer);
                }
            }
            
            return signers.ToArray();
        }
    }
}