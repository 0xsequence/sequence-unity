using System;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Authentication.Requests;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives.Common;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWallet
    {
        public static Action<SequenceEcosystemWallet> OnWalletCreated;
        
        public Address Address { get; }
        
        public SequenceEcosystemWallet(Address address)
        {
            Address = address;
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

            var redirectHandler = RedirectFactory.CreateHandler();
            var response = await redirectHandler.WaitForResponse<SignMessageArgs, SignMessageResponse>(url, "signMessage", args);
            
            if (!response.Result)
                throw new Exception("Failed to sign message");
            
            return response.Data;
        }
        
        public async Task SendTransaction()
        {
            throw new NotImplementedException();
        }
    }
}