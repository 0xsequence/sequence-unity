using System;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Relayer;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet
{
    public class SessionSigner : IWallet
    {
        public Address ParentAddress { get; }
        public Address Address { get; }
        public Chain Chain { get; }
        public bool IsExplicit { get; }

        private readonly SessionCredentials _credentials;
        
        internal SessionSigner(SessionCredentials credentials)
        {
            _credentials = credentials;

            ParentAddress = credentials.address;
            Address = new EOAWallet(credentials.privateKey).GetAddress();
            Chain = ChainDictionaries.ChainById[credentials.chainId];
            IsExplicit = credentials.isExplicit;
        }
        
        public async Task<SignMessageResponse> SignMessage(string message)
        {
            var args = new SignMessageArgs 
            { 
                address = Address, 
                chainId = new BigInt((int)Chain), 
                message = message
            };

            var ecosystem = (EcosystemType)_credentials.ecosystemId;
            var url = $"{EcosystemBindings.GetUrl(ecosystem)}/request/sign";

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
            var relayer = new SequenceRelayer(Chain.TestnetArbitrumSepolia);

            var args = new FeeOptionsArgs(Address, signedCalls.To, signedCalls.Data);
            var response = await relayer.GetFeeOptions(args);

            return response.options;
        }

        public async Task<string> SendTransaction(Call[] calls, FeeOption feeOption = null)
        {
            var signedCalls = await SignCalls(calls);
            
            var relayer = new SequenceRelayer(Chain);
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
            throw new System.NotImplementedException();
        }
    }
}