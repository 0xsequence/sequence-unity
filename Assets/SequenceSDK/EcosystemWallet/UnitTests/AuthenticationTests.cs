using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Authentication;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class AuthenticationTests
    {
        private static readonly Chain _chain = Chain.TestnetArbitrumSepolia;
        private readonly SequenceEcosystemWalletLogin _login = new(_chain);
        
        [TestCase("agru@horizon.io", "implicit")]
        [TestCase("agru@horizon.io", "explicit_open")]
        [TestCase("agru@horizon.io", "explicit_restrictive")]
        public async Task SignInWithEmailTest(string email, string sessionType)
        {
            var wallet = await _login.SignInWithEmail(email, GetPermissionsFromInput(sessionType));
            CheckStoredWallet(wallet);
        }
        
        [TestCase("implicit")]
        [TestCase("explicit_open")]
        [TestCase("explicit_restrictive")]
        public async Task SignInWithGoogleTest(string sessionType)
        {
            var wallet = await _login.SignInWithGoogle(GetPermissionsFromInput(sessionType));
            CheckStoredWallet(wallet);
        }
        
        [TestCase("implicit")]
        [TestCase("explicit_open")]
        [TestCase("explicit_restrictive")]
        public async Task SignInWithAppleTest(string sessionType)
        {
            var wallet = await _login.SignInWithApple(GetPermissionsFromInput(sessionType));
            CheckStoredWallet(wallet);
        }

        private void CheckStoredWallet(SequenceEcosystemWallet wallet)
        {
            var storedWallet = _login.RecoverSessionFromStorage();
            Assert.AreEqual(storedWallet.Address, wallet.Address);
        }

        private SessionPermissions GetPermissionsFromInput(string sessionType)
        {
            var templates = new SessionTemplates(_chain);
            return sessionType switch
            {
                "implicit" => null,
                "explicit_open" => templates.BuildUnrestrictivePermissions(),
                "explicit_restrictive" => templates.BuildBasicRestrictivePermissions(),
                _ => throw new Exception("Invalid session type"),
            };
        }
    }
}