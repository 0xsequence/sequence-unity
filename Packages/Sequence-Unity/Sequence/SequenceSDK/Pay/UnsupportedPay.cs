using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Pay.Sardine;
using Sequence.Pay.Transak;

namespace Sequence.Pay
{
    // If there is no supported Fiat payment method for the user and the given collectible, we use this class and throw a NotSupportedException
    internal class UnsupportedPay : IFiatPay
    {
        private Exception CreateException(object[] args, [CallerMemberName] string methodName = "")
        {
            string argsString = args != null && args.Length > 0 ? string.Join(", ", args) : "No arguments";
            return new NotSupportedException($"Method '{methodName}' is not supported. Arguments: {argsString}");
        }

        public Task OnRamp()
        {
            throw CreateException(null);
        }

        public Task<string> GetOnRampLink()
        {
            throw CreateException(null);
        }

        public Task<string> GetNftCheckoutLink(CollectibleOrder order, ulong amount, NFTType nftType = NFTType.ERC721,
            Address recipient = null, AdditionalFee additionalFee = null,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract)
        {
            throw CreateException(new object[] { order, amount, nftType, recipient, additionalFee, marketplaceContractAddress });
        }

        public void ConfigureSaleContractId(TransakContractId contractId)
        {
            throw CreateException(new object[] { contractId });
        }

        public Task<string> GetNftCheckoutLink(ERC1155Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, byte[] data = null, FixedByte[] proof = null)
        {
            throw CreateException(new object[] { saleContract, collection, tokenId, amount, recipient, data, proof });
        }

        public Task<string> GetNftCheckoutLink(ERC721Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, byte[] proof = null)
        {
            throw CreateException(new object[] { saleContract, collection, tokenId, amount, recipient, proof });
        }

        public Task<string> GetNftCheckoutLink(CollectibleOrder[] orders, BigInteger quantity, NFTType nftType = NFTType.ERC721,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract, Address recipient = null,
            AdditionalFee[] additionalFee = null)
        {
            throw CreateException(new object[] { orders, quantity, nftType, marketplaceContractAddress, recipient, additionalFee });
        }
    }
}