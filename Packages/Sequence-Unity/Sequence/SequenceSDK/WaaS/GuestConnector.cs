using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public class GuestConnector
    {
        private string _sessionId;
        private Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;

        public GuestConnector(string sessionId, Wallet.IWallet sessionWallet, IWaaSConnector connector)
        {
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _connector = connector;
        }

        public async Task ConnectToWaaSViaGuest()
        {
            IntentDataInitiateAuth initiateAuthIntent = AssembleGuestInitiateAuthIntent();

            string challenge = await _connector.InitiateAuth(initiateAuthIntent);
            
            IntentDataOpenSession loginIntent = AssembleGuestOpenSessionIntent(challenge);

            await _connector.ConnectToWaaS(loginIntent, LoginMethod.Guest);
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
    }
}