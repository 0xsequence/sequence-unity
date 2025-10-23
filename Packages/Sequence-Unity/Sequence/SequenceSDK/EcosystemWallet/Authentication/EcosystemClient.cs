using System;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet
{
    public enum SessionCreationType
    {
        CreateNewSession,
        IncludeImplicit,
        AddExplicit
    }
    
    internal class EcosystemClient
    {
        private readonly string _walletUrl;
        
        public EcosystemClient()
        {
            _walletUrl = SequenceConfig.GetConfig().WalletAppUrl;
            _walletUrl = _walletUrl.RemoveTrailingSlash();
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="addExplicit">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="session">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        public async Task<SessionSigner[]> CreateNewSession(bool addExplicit, SessionPermissions session, 
            string preferredLoginMethod, string email = null)
        {
            var chainId = string.Empty;
            if (session != null)
            {
                if (session.chainId <= 0)
                    throw new Exception("Invalid chainId.");
                
                chainId = session.chainId.ToString();
            }

            var type = DetermineSessionCreationType(addExplicit, session);
            var sessionWallet = new EOAWallet();
            var sessionAddress = sessionWallet.GetAddress();
            
            if (session != null)
                session.sessionAddress = sessionAddress;
            
            var sessionArgs = session == null ? 
                new SessionArgs(sessionAddress) : 
                new ExplicitSessionArgs(session);
            
            var origin = RedirectOrigin.GetOriginString();
            var includeImplicitSession = type == SessionCreationType.IncludeImplicit;
            
            var payload = new ConnectArgs
            {
                preferredLoginMethod = preferredLoginMethod,
                email = email,
                origin = origin,
                includeImplicitSession = includeImplicitSession,
                session = sessionArgs
            };
            
            var action = type == SessionCreationType.AddExplicit ? "addExplicitSession" : "createNewSession";
            
            var response = await SendRequest<ConnectArgs, ConnectResponse>("connect", action, payload);
            
            var isImplicitWithPermissions = includeImplicitSession && session != null;
            var credentialsLen = isImplicitWithPermissions ? 2 : 1;
            var credentials = new SessionCredentials[credentialsLen];

            var walletAddress = response.walletAddress;
            var guardConfig = response.guard;
            if (guardConfig != null)
                new GuardStorage().SaveConfig(walletAddress, guardConfig);
            
            credentials[0] = new SessionCredentials(
                !includeImplicitSession,
                sessionWallet.GetPrivateKeyAsHex(),
                walletAddress,
                response.attestation,
                response.signature,
                _walletUrl,
                chainId,
                response.loginMethod,
                response.userEmail);

            if (isImplicitWithPermissions)
            {
                credentials[1] = new SessionCredentials(
                    true,
                    sessionWallet.GetPrivateKeyAsHex(),
                    response.walletAddress,
                    null,
                    null,
                    _walletUrl,
                    chainId,
                    response.loginMethod,
                    response.userEmail);
            }
            
            var signers = new SessionSigner[credentials.Length];
            for (var i = 0; i < credentials.Length; i++)
            {
                var credential = credentials[i];
                signers[i] = new SessionSigner(credential);
                SessionStorage.AddSession(credential);
            }

            return signers;
        }

        public async Task<TResponse> SendRequest<TArgs, TResponse>(string path, string action, TArgs args)
        {
            var origin = RedirectOrigin.GetOriginString();
            
            var walletUrl = SequenceConfig.GetConfig().WalletAppUrl;
            walletUrl = walletUrl.RemoveTrailingSlash();
            
            var url = $"{walletUrl}/request/{path}";

            var handler = RedirectFactory.CreateHandler();
            handler.SetRedirectUrl(origin);
            
            var response = await handler.WaitForResponse<TArgs, TResponse>(url, action, args);
            if (!response.Result)
                throw new Exception("Error during request");
            
            return response.Data;
        }

        private SessionCreationType DetermineSessionCreationType(bool addExplicit, SessionPermissions permissions)
        {
            if (addExplicit)
                return SessionCreationType.AddExplicit;
            
            return permissions == null ? SessionCreationType.IncludeImplicit : SessionCreationType.CreateNewSession;
        }
    }
}