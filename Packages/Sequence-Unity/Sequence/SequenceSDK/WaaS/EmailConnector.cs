using System;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    internal class EmailConnector
    {
        private string _sessionId;
        private Wallet.IWallet _sessionWallet;
        private IWaaSConnector _connector;
        private IValidator _validator;
        private string _emailChallenge;

        public EmailConnector(string sessionId, Wallet.IWallet sessionWallet, IWaaSConnector connector, IValidator validator)
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
            string answerPreHash = _emailChallenge + code;
            string answer = SequenceCoder.KeccakHashASCII(answerPreHash).EnsureHexPrefix();
            string verifier = GetVerifier(email, _sessionId);
            IntentDataOpenSession loginIntent = new IntentDataOpenSession(_sessionWallet.GetAddress(), IdentityType.Email, verifier, answer);
            await _connector.ConnectToWaaS(loginIntent, LoginMethod.Email, email);
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
    }
}