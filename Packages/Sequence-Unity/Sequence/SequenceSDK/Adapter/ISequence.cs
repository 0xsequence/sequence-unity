using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;

namespace Sequence.Adapter
{
    public interface ISequence
    {
        public Chain Chain { get; }
        
        /// <summary>
        /// The underlying Sequence Embedded Wallet reference. Use it for more control, such as transaction batches. 
        /// </summary>
        public IWallet Wallet { get; }
        
        // ONBOARDING
        
        /// <summary>
        /// Recover a wallet from secure storage.
        /// </summary>
        /// <returns>'true' if a stored wallet was found or 'false' if not or if session recovery is disabled in your SequenceConfig file.</returns>
        Task<bool> TryRecoverWalletFromStorage();

        /// <summary>
        /// Login using Email OTP. You will receive a code to your email. Use it with the 'ConfirmEmailCode' function to finish the login process.
        /// </summary>
        /// <param name="email">Email of the given user.</param>
        /// <returns></returns>
        Task<bool> EmailLogin(string email);

        /// <summary>
        /// You receive a code after calling 'EmailLogin'. Use it with this function to complete the login process.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<bool> ConfirmEmailCode(string email, string code);

        /// <summary>
        /// Login as a guest. When the user uninstall the application, they lose access to a guest wallet, unless they use our Account Federation feature to link the guest wallet to another login option.
        /// </summary>
        /// <returns></returns>
        Task<bool> GuestLogin();

        /// <summary>
        /// Sign In with Google. The user is redirected to an external browser.
        /// </summary>
        Task<bool> GoogleLogin();

        /// <summary>
        /// Sign In with Apple. The user is redirected to an external browser. On iOS, this function uses the native Sign In SDK.
        /// </summary>
        Task<bool> AppleLogin();

        Task<bool> SignOut();

        /// <summary>
        /// Get an id token as a JWT you can use to verify the user on your backend. Use any JWKS library to verify this token.
        /// </summary>
        /// <returns>JWT Id Token</returns>
        Task<string> GetIdToken();
        
        // POWER

        /// <summary>
        /// Get the native token balance for your local user. For example, the native token on the Ethereum Mainnet is the amount of 'ETH'
        /// </summary>
        /// <returns>Balance in wei.</returns>
        Task<BigInteger> GetMyNativeTokenBalance();

        /// <summary>
        /// Get the token balance for your local user.
        /// </summary>
        /// <param name="tokenAddress">Address of your token contract. This could be an ERC20, ERC1155, or ERC721 contract you deployed on https//sequence.build/</param>
        /// <returns>Balance in wei and the token metadata.</returns>
        Task<(BigInteger Balance, TokenMetadata TokenMetadata)> GetMyTokenBalance(Address tokenAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenAddress"></param>
        /// <returns></returns>
        Task<TokenSupply[]> GetTokenSupplies(Address tokenAddress);

        /// <summary>
        /// Send any ERC20, ERC1155 or ERC721 token to another wallet.
        /// </summary>
        /// <param name="recipientAddress">The address of the wallet you want to send the token to.</param>
        /// <param name="tokenAddress">The address of your token. For example one you have deployed through https://sequence.build/</param>
        /// <param name="tokenId">Leave it blank for ERC20 contracts.</param>
        /// <param name="amount">Leave it blank for ERC721 contracts.</param>
        /// <returns>Transaction receipt used to check transaction information onchain.</returns>
        /// <exception cref="Exception"></exception>
        Task<string> SendToken(Address recipientAddress, Address tokenAddress, string tokenId, BigInteger amount);
        
        // MONETIZATION

        /// <summary>
        /// Swap one of your tokens to another one. Make sure you have configured enough liquidity on a DEX such as UniSwap.
        /// </summary>
        /// <param name="sellToken"></param>
        /// <param name="buyToken"></param>
        /// <param name="buyAmount"></param>
        /// <returns></returns>
        Task<string> SwapToken(Address sellToken, Address buyToken, BigInteger buyAmount);

        /// <summary>
        /// Get all listings from your Marketplace on a given collection. Please make sure you have configured your Marketplace on https://sequence.build/
        /// </summary>
        /// <param name="collectionAddress"></param>
        /// <returns></returns>
        Task<CollectibleOrder[]> GetAllListingsFromMarketplace(Address collectionAddress);

        /// <summary>
        /// Create a listing for a given token you own. Please make sure you have configured your Marketplace on https://sequence.build/
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="currencyAddress"></param>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <param name="pricePerToken"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<string> CreateListingOnMarketplace(Address contractAddress, Address currencyAddress,
            string tokenId, BigInteger amount, BigInteger pricePerToken, DateTime expiry);

        /// <summary>
        /// Purchase an order from your Marketplace. Please make sure you have configured your Marketplace on https://sequence.build/
        /// </summary>
        /// <param name="order">The order as returned from the 'GetAllListingsFromMarketplace' function.</param>
        /// <param name="amount">The amount of orders you wish to purchase.</param>
        /// <returns></returns>
        Task<string> PurchaseOrderFromMarketplace(Order order, BigInteger amount);
    }
}