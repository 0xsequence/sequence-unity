using System;
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
        
        private SessionSigner FindSignersForCalls(Calls calls)
        {
            return SessionSigners[0];
        }
    }
}