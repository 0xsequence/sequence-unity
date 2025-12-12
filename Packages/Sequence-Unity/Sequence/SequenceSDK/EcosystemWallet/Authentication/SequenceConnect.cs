using System.Threading.Tasks;

namespace Sequence.EcosystemWallet
{
    public class SequenceConnect : IConnect
    {
        private readonly EcosystemClient _client = new();
        
        public async Task<IWallet> SignInWithEmail(string email, IPermissions permissions)
        {
            return await SignIn(permissions, "email", email);
        }
        
        public async Task<IWallet> SignInWithGoogle(IPermissions permissions)
        {
            return await SignIn(permissions, "google", null);
        }
        
        public async Task<IWallet> SignInWithApple(IPermissions permissions)
        {
            return await SignIn(permissions, "apple", null);
        }
        
        public async Task<IWallet> SignInWithPasskey(IPermissions permissions)
        {
            return await SignIn(permissions, "passkey", null);
        }
        
        public async Task<IWallet> SignInWithMnemonic(IPermissions permissions)
        {
            return await SignIn(permissions, "mnemonic", null);
        }

        private async Task<IWallet> SignIn(IPermissions permissions, string preferredLoginMethod, string email)
        {
            var signers = await _client.CreateNewSession(false, permissions?.GetPermissions(), preferredLoginMethod, email);
            return new SequenceWallet(signers);
        }
    }
}