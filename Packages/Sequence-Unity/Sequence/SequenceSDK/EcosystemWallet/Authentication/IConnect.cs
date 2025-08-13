using System.Threading.Tasks;

namespace Sequence.EcosystemWallet
{
    public interface IConnect
    {
        /// <summary>
        /// Sign in using email through an external browser.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<IWallet> SignInWithEmail(string email, IPermissions permissions = null);
        
        /// <summary>
        /// Sign in with Google through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<IWallet> SignInWithGoogle(IPermissions permissions = null);
        
        /// <summary>
        /// Sign in with Apple through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<IWallet> SignInWithApple(IPermissions permissions = null);
        
        /// <summary>
        /// Sign in with a passkey through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<IWallet> SignInWithPasskey(IPermissions permissions = null);
        
        /// <summary>
        /// Sign in using a mnemonic through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<IWallet> SignInWithMnemonic(IPermissions permissions = null);
    }
}