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
        
        public async Task ConnectToWaaSViaSocialLogin(LoginMethod method)
        {
            IntentDataInitiateAuth initiateAuthIntent = AssembleOIDCInitiateAuthIntent(_sessionId);

            await _connector.InitiateAuth(initiateAuthIntent, method);
            
            IntentDataOpenSession loginIntent = AssembleOIDCOpenSessionIntent(_idToken, _sessionWallet);

            string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(_idToken).email;
            await _connector.ConnectToWaaS(loginIntent, method, email);
        }
        
        private IntentDataInitiateAuth AssembleOIDCInitiateAuthIntent(string sessionId)
        {
            string verifier = GetVerifier();
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.OIDC, sessionId, verifier);
            return intent;
        }

        private string GetVerifier()
        {
            IdTokenJwtPayload idTokenPayload = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(_idToken);
            string idTokenHash = SequenceCoder.KeccakHashASCII(_idToken).EnsureHexPrefix();
            return $"{idTokenHash};{idTokenPayload.exp}";
        }

        private IntentDataOpenSession AssembleOIDCOpenSessionIntent(string idToken, Wallet.IWallet sessionWallet)
        {
            string verifier = GetVerifier();
            IntentDataOpenSession intent =
                new IntentDataOpenSession(sessionWallet.GetAddress(), IdentityType.OIDC, verifier, idToken);
            return intent;
        }
    }
}