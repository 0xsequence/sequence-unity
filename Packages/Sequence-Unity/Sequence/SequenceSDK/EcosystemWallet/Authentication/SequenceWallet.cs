using System;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Relayer;

namespace Sequence.EcosystemWallet
{
    public class SequenceWallet : IWallet, IDisposable
    {
        public static Action<SequenceWallet> OnWalletCreated;
        
        public Address Address { get; }
        public SessionSigner[] SessionWallets { get; private set; }
        
        internal SequenceWallet(SessionSigner[] sessionWallets)
        {
            SessionWallets = sessionWallets;
            Address = sessionWallets[0].ParentAddress;
            OnWalletCreated?.Invoke(this);
            
            SequenceConnect.SessionsChanged += SessionsChanged;
        }

        public void Dispose()
        {
            SequenceConnect.SessionsChanged -= SessionsChanged;
        }

        private void SessionsChanged(SessionSigner[] sessionWallets)
        {
            SessionWallets = sessionWallets;
        }

        public async Task<SignMessageResponse> SignMessage(string message)
        {
            return await SessionWallets[0].SignMessage(message);
        }
        
        public async Task<string> SendTransaction(Call[] calls, FeeOption feeOption = null)
        {
            return await SessionWallets[0].SendTransaction(calls, feeOption);
        }

        public async Task<FeeOption[]> GetFeeOption(Call[] calls)
        {
            return await SessionWallets[0].GetFeeOption(calls);
        }
    }
}