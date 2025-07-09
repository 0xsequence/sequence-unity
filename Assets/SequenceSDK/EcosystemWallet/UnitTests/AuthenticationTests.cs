using NUnit.Framework;
using Sequence.EcosystemWallet.Authentication;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class AuthenticationTests
    {
        private readonly SequenceEcosystemWalletLogin _login = new(Chain.TestnetArbitrumSepolia);
        
        [TestCase("agru@horizon.io")]
        public void SignInWithEmailTest(string email)
        {
            _login.SignInWithEmail(email);
        }
    }
}