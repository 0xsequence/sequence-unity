using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    public class GuestConnector
    {
        private string _sessionId;
        private Sequence.Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;

        public GuestConnector(string sessionId, Sequence.Wallet.IWallet sessionWallet, IWaaSConnector connector)
        {
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _connector = connector;
        }

        public async Task ConnectToWaaSViaGuest()
        {
            string challenge = await InitiateAuth();
            
            IntentDataOpenSession loginIntent = AssembleGuestOpenSessionIntent(challenge);

            await _connector.ConnectToWaaS(loginIntent, LoginMethod.Guest);
        }
        
        private async Task<string> InitiateAuth()
        {
            IntentDataInitiateAuth intent = AssembleGuestInitiateAuthIntent();
            return await _connector.InitiateAuth(intent, LoginMethod.Guest);
        }
        
        private IntentDataInitiateAuth AssembleGuestInitiateAuthIntent()
        {
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.Guest, _sessionId, _sessionId);
            return intent;
        }

        private IntentDataOpenSession AssembleGuestOpenSessionIntent(string challenge)
        {
            string answerPreHash = challenge + _sessionId;
            string answer = SequenceCoder.KeccakHashASCII(answerPreHash).EnsureHexPrefix();
            IntentDataOpenSession intent =
                new IntentDataOpenSession(_sessionWallet.GetAddress(), IdentityType.Guest, _sessionId, answer);
            return intent;
        }
        
        private IntentDataFederateAccount AssembleGuestFederateAccountIntent(string challenge, string walletAddress)
        {
            IntentDataFederateAccount intent = new IntentDataFederateAccount(AssembleGuestOpenSessionIntent(challenge), walletAddress);
            return intent;
        }
    }
}