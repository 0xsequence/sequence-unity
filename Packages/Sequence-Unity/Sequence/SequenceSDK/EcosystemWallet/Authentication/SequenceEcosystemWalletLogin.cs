using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<SessionCredentials> _credentials = new();
        
        public SequenceEcosystemWalletLogin(Chain chain)
        {
            _chain = chain;
            _sessionStorage = new SessionStorage();
            _credentials = _sessionStorage.GetSessions().ToList();
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
            return GetAllSessions();
        }

        public SequenceEcosystemWallet[] GetAllSessions()
        {
            if (_credentials.Count == 0)
                throw new Exception("No session found in storage.");

            var wallets = new SequenceEcosystemWallet[_credentials.Count];
            for (var i = 0; i < _credentials.Count; i++)
                wallets[i] = new SequenceEcosystemWallet(_credentials[i]);

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

            var redirectUrl = RedirectOrigin.GetOriginString();
            var payload = new ConnectArgs
            {
                sessionAddress = _sessionWallet.GetAddress(),
                preferredLoginMethod = preferredLoginMethod,
                email = email,
                implicitSessionRedirectUrl = isExplicit ? null : redirectUrl,
                permissions = permissions
            };
            
            var action = isExplicit ? "addExplicitSession" : "addImplicitSession";
            var url = $"{WalletUrl}/request/connect";

            var handler = RedirectFactory.CreateHandler();
            handler.SetRedirectUrl(redirectUrl);
            
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
            _credentials.Add(credentials);
            
            return new SequenceEcosystemWallet(credentials);
        }
    }
}