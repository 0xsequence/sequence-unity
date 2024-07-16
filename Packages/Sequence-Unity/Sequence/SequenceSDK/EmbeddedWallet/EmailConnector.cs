using System;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    internal class EmailConnector
    {
        private string _sessionId;
        private Sequence.Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;
        private IValidator _validator;
        private string _emailChallenge;

        public EmailConnector(string sessionId, Sequence.Wallet.IWallet sessionWallet, IWaaSConnector connector, IValidator validator)
        {
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _connector = connector;
            _validator = validator;
        }

        private async Task<string> InitiateAuthEmail(string email)
        {
            IntentDataInitiateAuth initiateAuthIntent = AssembleEmailInitiateAuthIntent(email);

            _emailChallenge = await _connector.InitiateAuth(initiateAuthIntent, LoginMethod.Email);
            return _emailChallenge;
        }

        public async Task ConnectToWaaSViaEmail(string email, string code)
        {
            IntentDataOpenSession loginIntent = AssembleEmailOpenSessionIntent(email, code);
            await _connector.ConnectToWaaS(loginIntent, LoginMethod.Email, email);
        }
        
        private IntentDataOpenSession AssembleEmailOpenSessionIntent(string email, string code)
        {
            string answerPreHash = _emailChallenge + code;
            string answer = SequenceCoder.KeccakHashASCII(answerPreHash).EnsureHexPrefix();
            string verifier = GetVerifier(email, _sessionId);
            IntentDataOpenSession loginIntent = new IntentDataOpenSession(_sessionWallet.GetAddress(), IdentityType.Email, verifier, answer);
            return loginIntent;
        }

        private IntentDataInitiateAuth AssembleEmailInitiateAuthIntent(string email)
        {
            string verifier = GetVerifier(email, _sessionId);
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.Email, _sessionId, verifier);
            return intent;
        }

        private string GetVerifier(string email, string sessionId)
        {
            return $"{email};{sessionId}";
        }

        public async Task Login(string email)
        {
            if (!_validator.ValidateEmail(email))
            {
                throw new Exception($"Invalid email: {email}");
                return;
            }
            
            try
            {
                string emailChallenge = await InitiateAuthEmail(email);
                if (string.IsNullOrEmpty(emailChallenge))
                {
                    throw new Exception( "Email challenge is missing from response from WaaS API");
                }
                
                if (emailChallenge.StartsWith("Error"))
                {
                    throw new Exception( emailChallenge);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public async Task FederateAccount(string email, string code)
        {
            IntentDataFederateAccount intent = AssembleEmailFederateAccountIntent(email, code);
            await _connector.FederateAccount(intent, LoginMethod.Email, email);
        }
        
        private IntentDataFederateAccount AssembleEmailFederateAccountIntent(string email, string code)
        {
            IntentDataFederateAccount intent = new IntentDataFederateAccount(AssembleEmailOpenSessionIntent(email, code), _sessionWallet.GetAddress());
            return intent;
        }
    }
}