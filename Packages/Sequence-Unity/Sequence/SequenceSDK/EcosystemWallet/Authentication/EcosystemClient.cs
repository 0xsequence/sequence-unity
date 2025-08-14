using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    internal class EcosystemClient
    {
        private readonly EcosystemType _ecosystem;
        
        public EcosystemClient(EcosystemType ecosystem)
        {
            _ecosystem = ecosystem;
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
        public async Task<SessionSigner[]> CreateNewSession(bool isExplicit, SessionPermissions permissions, string preferredLoginMethod, string email = null)
        {
            var chainId = string.Empty;
            if (permissions != null)
            {
                if (permissions.chainId <= 0)
                    throw new Exception("Invalid chainId.");
                
                chainId = permissions.chainId.ToString();
            }
            
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

            var isImplicitWithPermissions = !isExplicit && permissions != null;
            var credentialsLen = isImplicitWithPermissions ? 2 : 1;
            var credentials = new SessionCredentials[credentialsLen];
            
            credentials[0] = new SessionCredentials(
                isExplicit,
                sessionWallet.GetPrivateKeyAsHex(),
                response.Data.walletAddress,
                response.Data.attestation,
                response.Data.signature,
                (int)_ecosystem,
                chainId,
                response.Data.loginMethod,
                response.Data.email);

            if (isImplicitWithPermissions)
            {
                credentials[1] = new SessionCredentials(
                    true,
                    sessionWallet.GetPrivateKeyAsHex(),
                    response.Data.walletAddress,
                    null,
                    null,
                    (int)_ecosystem,
                    chainId,
                    response.Data.loginMethod,
                    response.Data.email);
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
    }
}