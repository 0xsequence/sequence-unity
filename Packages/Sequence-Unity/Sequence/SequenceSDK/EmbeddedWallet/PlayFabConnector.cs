using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public class PlayFabConnector
    {
        private string _titleId;
        private string _sessionTicket;
        private string _sessionId;
        private Sequence.Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;
        
        public PlayFabConnector(string titleId, string sessionTicket, string sessionId, Sequence.Wallet.IWallet sessionWallet, IWaaSConnector connector)
        {
            _titleId = titleId;
            _sessionTicket = sessionTicket;
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _connector = connector;
        }
        
        public async Task ConnectToWaaSViaPlayFab(string email)
        {
            await InitiateAuth();
            
            IntentDataOpenSession loginIntent = AssemblePlayFabOpenSessionIntent();

            await _connector.ConnectToWaaS(loginIntent, LoginMethod.PlayFab, email);
        }
        
        private async Task InitiateAuth()
        {
            IntentDataInitiateAuth intent = AssemblePlayFabInitiateAuthIntent();
            await _connector.InitiateAuth(intent, LoginMethod.PlayFab);
        }
        
        private IntentDataInitiateAuth AssemblePlayFabInitiateAuthIntent()
        {
            string verifier = GetVerifier();
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.PlayFab, _sessionId, verifier);
            return intent;
        }
        
        private string GetVerifier()
        {
            string sessionTicketHash = SequenceCoder.KeccakHashASCII(_sessionTicket).EnsureHexPrefix();
            return $"{_titleId}|{sessionTicketHash}";
        }
        
        private IntentDataOpenSession AssemblePlayFabOpenSessionIntent()
        {
            string verifier = GetVerifier();
            IntentDataOpenSession intent =
                new IntentDataOpenSession(_sessionWallet.GetAddress(), IdentityType.PlayFab, verifier, _sessionTicket);
            return intent;
        }
        
        public async Task FederateAccount(string email)
        {
            await InitiateAuth();
            
            IntentDataFederateAccount federateIntent = AssemblePlayFabFederateAccountIntent();
            await _connector.FederateAccount(federateIntent, LoginMethod.PlayFab, email);
        }
        
        private IntentDataFederateAccount AssemblePlayFabFederateAccountIntent()
        {
            string verifier = GetVerifier();
            IntentDataFederateAccount intent = new IntentDataFederateAccount(_sessionId, _sessionWallet.GetAddress(), IdentityType.PlayFab.ToString(), verifier, _sessionTicket);
            return intent;
        }
    }
}