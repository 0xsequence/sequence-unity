using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class SequenceConnect : IConnect
    {
        private readonly EcosystemClient _client;
        
        public SequenceConnect(EcosystemType ecosystem)
        {
            _client = new EcosystemClient(ecosystem);
        }
        
        public async Task<IWallet> SignInWithEmail(string email, IPermissions permissions = null)
        {
            return await SignIn(permissions, "email", email);
        }
        
        public async Task<IWallet> SignInWithGoogle(IPermissions permissions = null)
        {
            return await SignIn(permissions, "google", null);
        }
        
        public async Task<IWallet> SignInWithApple(IPermissions permissions = null)
        {
            return await SignIn(permissions, "apple", null);
        }
        
        public async Task<IWallet> SignInWithPasskey(IPermissions permissions = null)
        {
            return await SignIn(permissions, "passkey", null);
        }
        
        public async Task<IWallet> SignInWithMnemonic(IPermissions permissions = null)
        {
            return await SignIn(permissions, "mnemonic", null);
        }

        private async Task<IWallet> SignIn(IPermissions permissions, string preferredLoginMethod, string email)
        {
            var signer = await _client.CreateNewSession(false, permissions.GetPermissions(), preferredLoginMethod, email);
            return new SequenceWallet(new [] { signer });
        }
    }
}