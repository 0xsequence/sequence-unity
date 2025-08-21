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
            var connect = new SequenceConnect(Ecosystem);
            await connect.SignInWithGoogle(null);
        }
        
        [Test]
        public async Task AddUnrestrictiveExplicitSession()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.AddSession(new ContractPermission(Chain, new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, 0));
        }
        
        [Test]
        public async Task AddRestrictiveExplicitSession()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.AddSession(new ContractPermission(Chain, new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, 0));
        }
    }
}