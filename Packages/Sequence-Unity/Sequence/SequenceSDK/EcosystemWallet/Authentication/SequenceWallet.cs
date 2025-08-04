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
            return await FindSessionSigner().SignMessage(message);
        }
        
        public async Task<string> SendTransaction(Call[] calls, FeeOption feeOption = null)
        {
            return await FindSessionSigner().SendTransaction(calls, feeOption);
        }

        public async Task<FeeOption[]> GetFeeOption(Call[] calls)
        {
            return await FindSessionSigner().GetFeeOption(calls);
        }

        private SessionSigner FindSessionSigner()
        {
            return SessionSigners[0];
        }
    }
}