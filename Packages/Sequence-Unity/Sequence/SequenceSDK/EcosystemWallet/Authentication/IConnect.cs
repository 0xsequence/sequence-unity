using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;

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
        Task<SequenceWallet> SignInWithEmail(string email, IPermissions permissions = null);
        
        /// <summary>
        /// Sign in with Google through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<SequenceWallet> SignInWithGoogle(IPermissions permissions = null);
        
        /// <summary>
        /// Sign in with Apple through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<SequenceWallet> SignInWithApple(IPermissions permissions = null);
        
        /// <summary>
        /// Sign in with a passkey through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<SequenceWallet> SignInWithPasskey(IPermissions permissions = null);
        
        /// <summary>
        /// Sign in using a mnemonic through an external browser.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<SequenceWallet> SignInWithMnemonic(IPermissions permissions = null);
    }
}