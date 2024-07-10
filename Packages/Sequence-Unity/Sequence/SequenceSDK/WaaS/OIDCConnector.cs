using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public class OIDCConnector
    {
        private string _idToken;
        private string _sessionId;
        private Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;

        public OIDCConnector(string idToken, string sessionId, Wallet.IWallet sessionWallet, IWaaSConnector connector)
        {
            _idToken = idToken;
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _connector = connector;
        }
        
        public async Task ConnectToWaaSViaSocialLogin(string idToken, LoginMethod method)
        {
            IntentDataInitiateAuth initiateAuthIntent = AssembleOIDCInitiateAuthIntent(idToken, _sessionId);

            await _connector.InitiateAuth(initiateAuthIntent);
            
            IntentDataOpenSession loginIntent = AssembleOIDCOpenSessionIntent(idToken, _sessionWallet);

            string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken).email;
            await _connector.ConnectToWaaS(loginIntent, method, email);
        }
        
        private IntentDataInitiateAuth AssembleOIDCInitiateAuthIntent(string idToken, string sessionId)
        {
            string verifier = GetVerifier(idToken);
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.OIDC, sessionId, verifier);
            return intent;
        }

        private string GetVerifier(string idToken)
        {
            IdTokenJwtPayload idTokenPayload = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken);
            string idTokenHash = SequenceCoder.KeccakHashASCII(idToken).EnsureHexPrefix();
            return $"{idTokenHash};{idTokenPayload.exp}";
        }

        private IntentDataOpenSession AssembleOIDCOpenSessionIntent(string idToken, Wallet.IWallet sessionWallet)
        {
            string verifier = GetVerifier(idToken);
            IntentDataOpenSession intent =
                new IntentDataOpenSession(sessionWallet.GetAddress(), IdentityType.OIDC, verifier, idToken);
            return intent;
        }
    }
}