using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Relayer;

namespace Sequence.EcosystemWallet
{
    public interface IWallet
    {
        /// <summary>
        /// The parent address of this wallet.
        /// </summary>
        Address Address { get; }
        
        /// <summary>
        /// Retrieves all signer addresses that you connected to your wallet.
        /// </summary>
        /// <returns></returns>
        Address[] GetAllSigners();
        
        /// <summary>
        /// Add sessions to extend your permissions.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task AddSession(Chain chain, SessionPermissions permissions);
        
        /// <summary>
        /// Clear all session signers from storage. This requires you to re-connect using the SequenceConnect object.
        /// </summary>
        void Disconnect();
        
        /// <summary>
        /// Sign a message through an external browser.
        /// </summary>
        /// <param name="message">The message you want to sign.</param>
        /// <returns></returns>
        Task<SignMessageResponse> SignMessage(string message);
        
        /// <summary>
        /// Get fee options for your calls. Only required for mainnets, if you don't have gas sponsorship configured.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="calls"></param>
        /// <returns></returns>
        Task<FeeOption[]> GetFeeOption(Chain chain, Call[] calls);
        
        /// <summary>
        /// Send a transaction.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="calls"></param>
        /// <param name="feeOption"></param>
        /// <returns></returns>
        Task<string> SendTransaction(Chain chain, Call[] calls, FeeOption feeOption = null);
    }
}