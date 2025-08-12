using System;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;

namespace Sequence.EcosystemWallet
{
    internal class EcosystemClient
    {
        private readonly EcosystemType _ecosystem;
        private readonly Chain _chain;
        
        public EcosystemClient(EcosystemType ecosystem, Chain chain)
        {
            _ecosystem = ecosystem;
            _chain = chain;
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="ecosystem">The ecosystem you want to connect with.</param>
        /// <param name="chain">The chain you want to connect with.</param>
        /// <param name="isExplicit">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        public async Task<SessionSigner> CreateNewSession(bool isExplicit, SessionPermissions permissions, string preferredLoginMethod, string email = null)
        {
            var sessionWallet = new EOAWallet();

            var origin = RedirectOrigin.GetOriginString();
            var payload = new ConnectArgs
            {
                sessionAddress = sessionWallet.GetAddress(),
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
            
            var credentials = new SessionCredentials(
                isExplicit,
                sessionWallet.GetPrivateKeyAsHex(),
                response.Data.walletAddress,
                response.Data.attestation,
                response.Data.signature,
                (int)_ecosystem,
                ChainDictionaries.ChainIdOf[_chain],
                response.Data.loginMethod,
                response.Data.email);

            SessionStorage.AddSession(credentials);
            return new SessionSigner(credentials);
        }
    }
}