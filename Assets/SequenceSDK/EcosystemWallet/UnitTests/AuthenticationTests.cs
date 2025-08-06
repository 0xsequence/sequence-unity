using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class AuthenticationTests
    {
        [Test]
        public async Task SignInWithGoogle()
        {
            var connect = new SequenceConnect(Chain.TestnetArbitrumSepolia, EcosystemType.Sequence);
            await connect.SignInWithGoogle();
        }
    }
}