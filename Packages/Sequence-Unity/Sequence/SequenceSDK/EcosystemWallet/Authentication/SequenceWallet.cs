using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Relayer;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    public class SequenceWallet : IWallet, IDisposable
    {
        public static Action<SequenceWallet> OnWalletCreated;
        
        public Address Address { get; }
        public SessionSigner[] SessionSigners { get; private set; }
        
        internal SequenceWallet(SessionSigner[] sessionSigners)
        {
            SessionSigners = sessionSigners;
            Address = sessionSigners[0].ParentAddress;
            OnWalletCreated?.Invoke(this);
            
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
            var signedCalls = await SignCalls(calls);
            var relayer = new SequenceRelayer(SessionSigners[0].Chain);

            var args = new FeeOptionsArgs(Address, signedCalls.To, signedCalls.Data);
            var response = await relayer.GetFeeOptions(args);

            return response.options;
        }

        public async Task<string> SendTransaction(Call[] calls, FeeOption feeOption = null)
        {
            var keyMachine = new KeyMachineApi();
            var deployResponse = await keyMachine.GetDeployHash(Address);
            var config = await keyMachine.GetConfiguration(deployResponse.deployHash);
            
            Debug.Log($"Config: {config.ToJson()}");

            var signerLeaf = config.topology.FindSignerLeaf(new Address("0x23c2eB9958BcAC9E531E785c4f65e91F1F426142")) as SapientSignerLeaf;
            var treeReturn = await keyMachine.GetTree(signerLeaf.imageHash);
            var sessionsTopology = SessionsTopology.FromTree(treeReturn.tree.ToString());
            
            Debug.Log($"Sessions Topology {sessionsTopology.JsonSerialize()}");
            
            return "";
            
            var signedCalls = await SignCalls(calls);
            
            var relayer = new SequenceRelayer(SessionSigners[0].Chain);
            var hash = await relayer.Relay(signedCalls.To, signedCalls.Data.ByteArrayToHexStringWithPrefix());

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
        
        private async Task<(Address To, byte[] Data)> SignCalls(Call[] calls)
        {
            /*
             * 1. Prepare Increment, add the increment to the calls array
             * 2. 
             */

            var preparedIncrement = PrepareIncrement(null, 2, null);
            if (preparedIncrement != null)
                calls.AddToArray(preparedIncrement);

            var envelope = PrepareTransaction(calls);
            var parentedEnvelope = new Parented(new [] { Address }, envelope.payload);
            
            // Get Image hash
            
            // sign sapient
            
            throw new System.NotImplementedException();
        }

        private Call PrepareIncrement(Address wallet, BigInteger chainId, Calls calls)
        {
            return null;
        }

        private Envelope<Calls> PrepareTransaction(Call[] calls)
        {
            var space = 0;
            var nonce = 0;
            
            return new Envelope<Calls>
            {
                wallet = null,
                payload = new Calls(space, nonce, calls)
            };
        }

        private void BuildTransaction(SignedEnvelope<Calls> envelope)
        {
        }

        private async Task<SignatureOfSapientSignerLeaf> SignSapient(Calls calls)
        {
            if (!calls.isCalls || calls.calls.Length == 0)
                throw new Exception("calls is empty");

            var deployHashReturn = await new KeyMachineApi().GetDeployHash(Address);
            var imageHash = deployHashReturn.deployHash;

            var implicitSigners = new List<Address>();
            var explicitSigners = new List<Address>();

            var signers = FindSignersForCalls(calls);
            foreach (var signer in signers)
            {
                if (signer.IsExplicit)
                    explicitSigners.Add(signer.Address);
                else
                    implicitSigners.Add(signer.Address);
            }

            var sessionSignatures = SessionCallSignature.EncodeSignatures(
                null,
                null, 
                explicitSigners.ToArray(), 
                implicitSigners.ToArray());
            
            return new SignatureOfSapientSignerLeaf
            {
                curType = SignatureOfSapientSignerLeaf.Type.sapient,
                address = Address,
                data = sessionSignatures
            };
        }
        
        private SessionSigner[] FindSignersForCalls(Calls calls)
        {
            return SessionSigners;
        }
    }
}