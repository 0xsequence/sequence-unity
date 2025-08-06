using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.EcosystemWallet
{
    public class SequenceConnect
    {
        public static Action<SessionSigner[]> SessionsChanged;
        
        public Chain Chain { get; private set; }
        
        private EcosystemType _ecosystem;
        private EOAWallet _sessionWallet;
        private SessionStorage _sessionStorage;
        private List<SessionCredentials> _credentials;
        private SequenceWallet _wallet;
        
        public SequenceConnect(Chain chain, EcosystemType ecosystem)
        {
            SetChain(chain);
            _ecosystem = ecosystem;
            _sessionStorage = new SessionStorage();
            _credentials = _sessionStorage.GetSessions().ToList();
        }

        public void SetChain(Chain chain)
        {
            Chain = chain;
        }

        public async Task<SessionSigner> AddSession(SessionPermissions permissions)
        {
            Assert.IsNotNull(permissions);
            return await CreateNewSession(true, permissions, string.Empty);
        }
        
        public async Task<SessionSigner> SignInWithEmail(string email, SessionPermissions permissions = null)
        {
            return await CreateNewSession(false, permissions,"email", email);
        }
        
        public async Task<SessionSigner> SignInWithGoogle(SessionPermissions permissions = null)
        {
            return await CreateNewSession(false, permissions,"google");
        }
        
        public async Task<SessionSigner> SignInWithApple(SessionPermissions permissions = null)
        {
            return await CreateNewSession(false, permissions,"apple");
        }
        
        public async Task<SessionSigner> SignInWithPasskey(SessionPermissions permissions = null)
        {
            return await CreateNewSession(false, permissions,"passkey");
        }
        
        public async Task<SessionSigner> SignInWithMnemonic(SessionPermissions permissions = null)
        {
            return await CreateNewSession(false, permissions,"mnemonic");
        }

        public SequenceWallet GetWallet()
        {
            _wallet ??= new SequenceWallet(GetAllSessionWallets());
            return _wallet;
        }

        public SessionSigner[] GetAllSessionWallets()
        {
            if (_credentials.Count == 0)
                return Array.Empty<SessionSigner>();

            var sessionWallets = new SessionSigner[_credentials.Count];
            for (var i = 0; i < _credentials.Count; i++)
                sessionWallets[i] = new SessionSigner(_credentials[i]);

            return sessionWallets;
        }

        public void SignOut()
        {
            _sessionStorage.Clear();
            _credentials.Clear();
            SessionsChanged?.Invoke(GetAllSessionWallets());
        }

        public void RemoveSession(Address sessionAddress)
        {
            var index = _credentials.FindIndex(c => c.sessionAddress.Equals(sessionAddress));
            if (index == -1)
                throw new Exception("");
            
            _credentials.RemoveAt(index);
            _sessionStorage.StoreSessions(_credentials.ToArray());
            SessionsChanged?.Invoke(GetAllSessionWallets());
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="isExplicit">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        private async Task<SessionSigner> CreateNewSession(bool isExplicit, SessionPermissions permissions, string preferredLoginMethod, string email = null)
        {
            _sessionWallet = new EOAWallet();

            var origin = RedirectOrigin.GetOriginString();
            var payload = new ConnectArgs
            {
                sessionAddress = _sessionWallet.GetAddress(),
                preferredLoginMethod = preferredLoginMethod,
                email = email,
                origin = origin,
                permissions = permissions
            };
            
            var action = isExplicit ? "addExplicitSession" : "addImplicitSession";
            var ecosystemUrl = EcosystemBindings.GetUrl(_ecosystem);
            var url = $"{ecosystemUrl}/request/connect";

            var handler = RedirectFactory.CreateHandler();
            handler.SetRedirectUrl(origin);
            
            var response = await handler.WaitForResponse<ConnectArgs, ConnectResponse>(url, action, payload);
            if (!response.Result)
                throw new Exception("Error during request");
            
            var keyMachine = new KeyMachineApi();
            var deployResponse = await keyMachine.GetDeployHash(response.Data.walletAddress);
            var config = await keyMachine.GetConfiguration(deployResponse.deployHash);
            
            Debug.Log($"Config: {config.ToJson()}");
            
            var credentials = new SessionCredentials(
                isExplicit,
                _sessionWallet.GetPrivateKeyAsHex(),
                response.Data.walletAddress,
                response.Data.attestation,
                response.Data.signature,
                (int)_ecosystem,
                ChainDictionaries.ChainIdOf[Chain],
                response.Data.loginMethod,
                response.Data.email);
            
            _sessionStorage.AddSession(credentials);
            _credentials.Add(credentials);
            
            SessionsChanged?.Invoke(GetAllSessionWallets());
            return new SessionSigner(credentials);
        }
    }
}