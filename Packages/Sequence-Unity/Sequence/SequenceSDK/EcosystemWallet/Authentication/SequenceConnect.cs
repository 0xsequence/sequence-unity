using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class SequenceConnect : IConnect
    {
        private EcosystemClient _client;
        
        public SequenceConnect(EcosystemType ecosystem, Chain chain)
        {
            _client = new EcosystemClient(ecosystem, chain);
        }
        
        public async Task<SequenceWallet> SignInWithEmail(string email, SessionPermissions permissions = null)
        {
            return await SignIn(permissions, "email", email);
        }
        
        public async Task<SequenceWallet> SignInWithGoogle(SessionPermissions permissions = null)
        {
            return await SignIn(permissions, "google", null);
        }
        
        public async Task<SequenceWallet> SignInWithApple(SessionPermissions permissions = null)
        {
            return await SignIn(permissions, "apple", null);
        }
        
        public async Task<SequenceWallet> SignInWithPasskey(SessionPermissions permissions = null)
        {
            return await SignIn(permissions, "passkey", null);
        }
        
        public async Task<SequenceWallet> SignInWithMnemonic(SessionPermissions permissions = null)
        {
            return await SignIn(permissions, "mnemonic", null);
        }

        private async Task<SequenceWallet> SignIn(SessionPermissions permissions, string preferredLoginMethod, string email)
        {
            var signer = await _client.CreateNewSession(false, permissions,preferredLoginMethod, email);
            return new SequenceWallet(new [] { signer });
        }
    }
}