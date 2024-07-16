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
        private Sequence.Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;

        public OIDCConnector(string idToken, string sessionId, Sequence.Wallet.IWallet sessionWallet, IWaaSConnector connector)
        {
            _idToken = idToken;
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _connector = connector;
        }
        
        public async Task ConnectToWaaSViaSocialLogin(LoginMethod method)
        {
            await InitiateAuth(method);
            
            IntentDataOpenSession loginIntent = AssembleOIDCOpenSessionIntent();

            string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(_idToken).email;
            await _connector.ConnectToWaaS(loginIntent, method, email);
        }
        
        private async Task InitiateAuth(LoginMethod method)
        {
            IntentDataInitiateAuth intent = AssembleOIDCInitiateAuthIntent();
            await _connector.InitiateAuth(intent, method);
        }

        private IntentDataInitiateAuth AssembleOIDCInitiateAuthIntent()
        {
            string verifier = GetVerifier();
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.OIDC, _sessionId, verifier);
            return intent;
        }

        private string GetVerifier()
        {
            IdTokenJwtPayload idTokenPayload = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(_idToken);
            string idTokenHash = SequenceCoder.KeccakHashASCII(_idToken).EnsureHexPrefix();
            return $"{idTokenHash};{idTokenPayload.exp}";
        }

        private IntentDataOpenSession AssembleOIDCOpenSessionIntent()
        {
            string verifier = GetVerifier();
            IntentDataOpenSession intent =
                new IntentDataOpenSession(_sessionWallet.GetAddress(), IdentityType.OIDC, verifier, _idToken);
            return intent;
        }

        public async Task FederateAccount(LoginMethod method)
        {
            await InitiateAuth(method);
            IntentDataFederateAccount intent = AssembleOIDCFederateAccountIntent();
            string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(_idToken).email;
            await _connector.FederateAccount(intent, method, email);
        }
        
        private IntentDataFederateAccount AssembleOIDCFederateAccountIntent()
        {
            IntentDataFederateAccount intent = new IntentDataFederateAccount(AssembleOIDCOpenSessionIntent(), _sessionWallet.GetAddress());
            return intent;
        }
    }
}