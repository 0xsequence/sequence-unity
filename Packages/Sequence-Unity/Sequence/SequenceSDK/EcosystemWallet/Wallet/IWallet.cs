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
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task AddSession(IPermissions permissions);
        
        /// <summary>
        /// Clear all session signers from storage. This requires you to re-connect using the SequenceConnect object.
        /// </summary>
        void Disconnect();
        
        /// <summary>
        /// Sign a message through an external browser.
        /// </summary>
        /// <param name="chain">The message you want to sign.</param>
        /// <param name="message">The message you want to sign.</param>
        /// <returns></returns>
        Task<SignMessageResponse> SignMessage(Chain chain, string message);
        
        /// <summary>
        /// Get fee options for your calls. Only required for mainnets, if you don't have gas sponsorship configured.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<FeeOption[]> GetFeeOption(Chain chain, ITransaction transaction);
        
        /// <summary>
        /// Get fee options for your calls. Only required for mainnets, if you don't have gas sponsorship configured.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        Task<FeeOption[]> GetFeeOption(Chain chain, ITransaction[] transactions);
        
        /// <summary>
        /// Send a single transaction.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="transaction"></param>
        /// <param name="feeOption"></param>
        /// <returns></returns>
        Task<string> SendTransaction(Chain chain, ITransaction transaction, FeeOption feeOption = null);
        
        /// <summary>
        /// Send a transaction.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="transactions"></param>
        /// <param name="feeOption"></param>
        /// <returns></returns>
        Task<string> SendTransaction(Chain chain, ITransaction[] transactions, FeeOption feeOption = null);

        /// <summary>
        /// Checks whether this wallet is capable of signing the given calls.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="transaction"></param>
        /// <returns>False if no local signer could be found.</returns>
        Task<bool> SupportsTransaction(Chain chain, ITransaction transaction);
        
        /// <summary>
        /// Checks whether this wallet is capable of signing the given calls.
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="transactions"></param>
        /// <returns>False if no local signer could be found.</returns>
        Task<bool> SupportsTransaction(Chain chain, ITransaction[] transactions);
    }
}