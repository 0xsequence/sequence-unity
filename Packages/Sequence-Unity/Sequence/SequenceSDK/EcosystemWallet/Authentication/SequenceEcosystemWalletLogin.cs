using System;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWalletLogin
    {
        internal const string WalletUrl = "https://v3.sequence-dev.app"; 
        
        private Chain _chain;
        private EOAWallet _sessionWallet;
        private SessionStorage _sessionStorage;
        
        public SequenceEcosystemWalletLogin(Chain chain)
        {
            _chain = chain;
            _sessionStorage = new SessionStorage();
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithEmail(string email, SessionPermissions permissions)
        {
            return await CreateNewSession(permissions,"email", email);
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithGoogle(SessionPermissions permissions)
        {
            return await CreateNewSession(permissions,"google");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithApple(SessionPermissions permissions)
        {
            return await CreateNewSession(permissions,"apple");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithPasskey(SessionPermissions permissions)
        {
            return await CreateNewSession(permissions,"passkey");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithMnemonic(SessionPermissions permissions)
        {
            return await CreateNewSession(permissions,"mnemonic");
        }

        public SequenceEcosystemWallet RecoverSessionFromStorage()
        {
            var walletAddress = _sessionStorage.GetWalletAddress();
            var sessions = _sessionStorage.GetSessions();

            if (string.IsNullOrEmpty(walletAddress) || sessions.Length == 0)
                throw new Exception("No session found in storage.");

            return new SequenceEcosystemWallet(new Address(walletAddress));
        }

        public void SignOut()
        {
            _sessionStorage.Clear();
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        private async Task<SequenceEcosystemWallet> CreateNewSession(SessionPermissions permissions, string preferredLoginMethod, string email = null)
        {
            _sessionWallet = new EOAWallet();
            
            var isImplicitSession = permissions == null;
            var payload = new ConnectArgs
            {
                sessionAddress = _sessionWallet.GetAddress(),
                preferredLoginMethod = preferredLoginMethod,
                email = email,
                implicitSessionRedirectUrl = isImplicitSession ? RedirectOrigin.GetOriginString() : null,
                permissions = isImplicitSession ? null : permissions.ToJson()
            };
            
            var action = isImplicitSession ? "addImplicitSession" : "addExplicitSession";
            var url = $"{WalletUrl}/request/connect";

            var handler = RedirectFactory.CreateHandler();
            var response = await handler.WaitForResponse<ConnectArgs, ConnectResponse>(url, action, payload);
            if (!response.Result)
                throw new Exception("Error during request");
            
            var walletAddress = response.Data.walletAddress;
            _sessionStorage.StoreWalletAddress(walletAddress);
            _sessionStorage.AddSession(new SessionData(
                _sessionWallet.GetPrivateKeyAsHex(), 
                walletAddress, 
                response.Data.attestation, 
                response.Data.signature,
                ChainDictionaries.ChainIdOf[_chain],
                response.Data.loginMethod,
                response.Data.email));
            
            return new SequenceEcosystemWallet(walletAddress);
        }
    }
}