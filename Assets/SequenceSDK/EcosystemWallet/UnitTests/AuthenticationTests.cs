using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class AuthenticationTests
    {
        private static readonly Chain Chain = Chain.TestnetArbitrumSepolia;
        private static readonly EcosystemType Ecosystem = EcosystemType.Sequence;
        
        [Test]
        public void Disconnect()
        {
            SequenceWallet.RecoverFromStorage().Disconnect();
        }
        
        [Test]
        public async Task SignInWithGoogle()
        {
            var connect = new SequenceConnect(Ecosystem, Chain);
            await connect.SignInWithGoogle();
        }
        
        [Test]
        public async Task AddUnrestrictiveExplicitSession()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.AddSession(Chain, new SessionTemplates(Chain).BuildUnrestrictivePermissions());
        }
        
        [Test]
        public async Task AddRestrictiveExplicitSession()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.AddSession(Chain, new SessionTemplates(Chain).BuildBasicRestrictivePermissions());
        }
    }
}