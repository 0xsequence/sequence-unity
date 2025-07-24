using System;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Authentication.Requests;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Relayer;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWallet
    {
        public static Action<SequenceEcosystemWallet> OnWalletCreated;
        
        public Address Address { get; }
        public Address SessionAddress { get; }
        public Chain Chain { get; }
        public bool IsExplicit { get; }

        private readonly SessionCredentials _credentials;
        
        internal SequenceEcosystemWallet(SessionCredentials credentials)
        {
            _credentials = credentials;
            
            Address = credentials.address;
            SessionAddress = new EOAWallet(credentials.privateKey).GetAddress();
            Chain = ChainDictionaries.ChainById[credentials.chainId];
            IsExplicit = credentials.isExplicit;
            OnWalletCreated?.Invoke(this);
        }

        public async Task<SignMessageResponse> SignMessage(Chain chain, string message)
        {
            var args = new SignMessageArgs 
            { 
                address = Address, 
                chainId = new BigInt((int)chain), 
                message = message
            };
            var url = $"{SequenceEcosystemWalletLogin.WalletUrl}/request/sign";

            var handler = RedirectFactory.CreateHandler();
            handler.SetRedirectUrl(RedirectOrigin.GetOriginString());
            
            var response = await handler.WaitForResponse<SignMessageArgs, SignMessageResponse>(url, "signMessage", args);
            
            if (!response.Result)
                throw new Exception("Failed to sign message");
            
            return response.Data;
        }
        
        public async Task SendTransaction(Call[] calls, FeeOption feeOption = null)
        {
            var signedCalls = await SignCalls(calls);
            throw new NotImplementedException();
        }

        public async Task<FeeOption[]> GetFeeOption(Call[] calls)
        {
            var signedCalls = await SignCalls(calls);
            throw new NotImplementedException();
        }

        private async Task<(Address To, byte[] Data)> SignCalls(Call[] calls)
        {
            throw new NotImplementedException();
        }
    }
}