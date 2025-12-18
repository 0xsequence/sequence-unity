using System.Threading.Tasks;
using Sequence.Config;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public class SequenceConnect : IConnect
    {
        private readonly EcosystemClient _client = new();

        public async Task<EcosystemConfig> GetEcosystemConfig()
        {
            var walletUrl= SequenceConfig.GetConfig().WalletAppUrl;
            var client = new HttpClient(walletUrl);
            return await client.SendGetRequest<EcosystemConfig>("api/wallet-configuration");
        }
        
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