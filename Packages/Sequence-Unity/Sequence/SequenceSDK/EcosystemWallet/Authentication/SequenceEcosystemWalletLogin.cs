using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWalletLogin
    {
        private Chain _chain;
        private string _walletUrl;
        private EOAWallet _sessionWallet;
        private SessionStorage _sessionStorage;
        
        public SequenceEcosystemWalletLogin(Chain chain)
        {
            _chain = chain;
            _walletUrl = "https://v3.sequence-dev.app";
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
            var payload = new Dictionary<string, object>();
            payload.Add("sessionAddress", _sessionWallet.GetAddress());
            payload.Add("preferredLoginMethod", preferredLoginMethod);
            payload.Add("email", email);
            
            if (isImplicitSession)
                payload.Add("implicitSessionRedirectUrl", RedirectOrigin.GetOriginString());
            
            if (!isImplicitSession)
                payload.Add("permissions", permissions.ToJson());

            var action = isImplicitSession ? "addImplicitSession" : "addExplicitSession";
            var url = $"{_walletUrl}/request/connect";

            var handler = RedirectFactory.CreateHandler();
            var response = await handler.WaitForResponse(url, action, payload);
            if (!response.Result)
            {
                throw new Exception("Error during request");
            }

            if (!response.QueryString.AllKeys.Contains("payload"))
            {
                var errorJson = Encoding.UTF8.GetString(Convert.FromBase64String(response.QueryString["error"]));
                var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorJson)["error"];
                
                Debug.LogError($"Error from wallet app: {error}");
                throw new Exception(error);
            }
            
            var encodedResponsePayload = response.QueryString["payload"];
            var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(encodedResponsePayload));
            var responsePayload = JsonConvert.DeserializeObject<AuthResponse>(responsePayloadJson);
            
            if (responsePayload.attestation != null)
                Debug.Log($"Attestation approvedSigner: {responsePayload.attestation.approvedSigner}");
            
            if (responsePayload.signature != null)
                Debug.Log($"Signature: {responsePayload.signature}");

            var walletAddress = responsePayload.walletAddress;
            _sessionStorage.StoreWalletAddress(walletAddress);
            _sessionStorage.AddSession(new SessionData(
                _sessionWallet.GetPrivateKeyAsHex(), 
                walletAddress, 
                responsePayload.attestation, 
                responsePayload.signature,
                ChainDictionaries.ChainIdOf[_chain],
                responsePayload.loginMethod,
                responsePayload.email));
            
            return new SequenceEcosystemWallet(walletAddress);
        }
    }
}