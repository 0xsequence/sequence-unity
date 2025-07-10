using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Authentication;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class AuthenticationTests
    {
        private readonly SequenceEcosystemWalletLogin _login = new(Chain.TestnetArbitrumSepolia);
        
        [TestCase("agru@horizon.io", "implicit")]
        [TestCase("agru@horizon.io", "explicit_open")]
        [TestCase("agru@horizon.io", "explicit_restrictive")]
        public async Task SignInWithEmailTest(string email, string sessionType)
        {
            var wallet = await _login.SignInWithEmail(email, ConvertToSessionType(sessionType));
            CheckStoredWallet(wallet);
        }
        
        [TestCase("implicit")]
        [TestCase("explicit_open")]
        [TestCase("explicit_restrictive")]
        public async Task SignInWithGoogleTest(string sessionType)
        {
            var wallet = await _login.SignInWithGoogle(ConvertToSessionType(sessionType));
            CheckStoredWallet(wallet);
        }
        
        [TestCase("implicit")]
        [TestCase("explicit_open")]
        [TestCase("explicit_restrictive")]
        public async Task SignInWithAppleTest(string sessionType)
        {
            var wallet = await _login.SignInWithApple(ConvertToSessionType(sessionType));
            CheckStoredWallet(wallet);
        }

        private void CheckStoredWallet(SequenceEcosystemWallet wallet)
        {
            var storedWallet = _login.RecoverSessionFromStorage();
            Assert.AreEqual(storedWallet.Address, wallet.Address);
        }

        private SequenceEcosystemWalletLogin.SessionType ConvertToSessionType(string sessionType)
        {
            return sessionType switch
            {
                "implicit" => SequenceEcosystemWalletLogin.SessionType.Implicit,
                "explicit_open" => SequenceEcosystemWalletLogin.SessionType.ExplicitOpen,
                "explicit_restrictive" => SequenceEcosystemWalletLogin.SessionType.ExplicitRestrictive,
                _ => throw new Exception("Invalid session type"),
            };
        }
    }
}