using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class SequenceConnect : IConnect
    {
        private readonly EcosystemClient _client;
        
        public SequenceConnect(EcosystemType ecosystem, Chain chain)
        {
            _client = new EcosystemClient(ecosystem, chain);
        }
        
        public async Task<SequenceWallet> SignInWithEmail(string email, IPermissions permissions = null)
        {
            return await SignIn(permissions, "email", email);
        }
        
        public async Task<SequenceWallet> SignInWithGoogle(IPermissions permissions = null)
        {
            return await SignIn(permissions, "google", null);
        }
        
        public async Task<SequenceWallet> SignInWithApple(IPermissions permissions = null)
        {
            return await SignIn(permissions, "apple", null);
        }
        
        public async Task<SequenceWallet> SignInWithPasskey(IPermissions permissions = null)
        {
            return await SignIn(permissions, "passkey", null);
        }
        
        public async Task<SequenceWallet> SignInWithMnemonic(IPermissions permissions = null)
        {
            return await SignIn(permissions, "mnemonic", null);
        }

        private async Task<SequenceWallet> SignIn(IPermissions permissions, string preferredLoginMethod, string email)
        {
            var signer = await _client.CreateNewSession(false, permissions.GetPermissions(), preferredLoginMethod, email);
            return new SequenceWallet(new [] { signer });
        }
    }
}