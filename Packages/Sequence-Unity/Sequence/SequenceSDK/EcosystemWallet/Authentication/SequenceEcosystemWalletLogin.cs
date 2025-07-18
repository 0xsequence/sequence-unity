using System;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;
using UnityEngine.Assertions;

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

        public async Task<SequenceEcosystemWallet> AddSession(SessionPermissions permissions)
        {
            Assert.IsNotNull(permissions);
            return await CreateNewSession(true, permissions, string.Empty);
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithEmail(string email, SessionPermissions permissions)
        {
            return await CreateNewSession(false, permissions,"email", email);
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithGoogle(SessionPermissions permissions)
        {
            return await CreateNewSession(false, permissions,"google");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithApple(SessionPermissions permissions)
        {
            return await CreateNewSession(false, permissions,"apple");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithPasskey(SessionPermissions permissions)
        {
            return await CreateNewSession(false, permissions,"passkey");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithMnemonic(SessionPermissions permissions)
        {
            return await CreateNewSession(false, permissions,"mnemonic");
        }

        public SequenceEcosystemWallet[] RecoverSessionsFromStorage()
        {
            var credentials = _sessionStorage.GetSessions();
            if (credentials.Length == 0)
                throw new Exception("No session found in storage.");

            var wallets = new SequenceEcosystemWallet[credentials.Length];
            for (var i = 0; i < credentials.Length; i++)
                wallets[i] = new SequenceEcosystemWallet(credentials[i]);

            return wallets;
        }

        public void SignOut()
        {
            _sessionStorage.Clear();
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="isExplicit">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        private async Task<SequenceEcosystemWallet> CreateNewSession(bool isExplicit, SessionPermissions permissions, string preferredLoginMethod, string email = null)
        {
            _sessionWallet = new EOAWallet();
            
            var payload = new ConnectArgs
            {
                sessionAddress = _sessionWallet.GetAddress(),
                preferredLoginMethod = preferredLoginMethod,
                email = email,
                implicitSessionRedirectUrl = isExplicit ? null : RedirectOrigin.GetOriginString(),
                permissions = permissions?.ToJson()
            };
            
            var action = isExplicit ? "addExplicitSession" : "addImplicitSession";
            var url = $"{WalletUrl}/request/connect";

            var handler = RedirectFactory.CreateHandler();
            var response = await handler.WaitForResponse<ConnectArgs, ConnectResponse>(url, action, payload);
            if (!response.Result)
                throw new Exception("Error during request");
            
            var credentials = new SessionCredentials(
                isExplicit,
                _sessionWallet.GetPrivateKeyAsHex(),
                response.Data.walletAddress,
                response.Data.attestation,
                response.Data.signature,
                ChainDictionaries.ChainIdOf[_chain],
                response.Data.loginMethod,
                response.Data.email);
            
            _sessionStorage.AddSession(credentials);
            return new SequenceEcosystemWallet(credentials);
        }
    }
}