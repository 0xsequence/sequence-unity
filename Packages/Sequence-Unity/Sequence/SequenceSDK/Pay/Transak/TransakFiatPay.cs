using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Pay.Sardine;
using UnityEngine;

namespace Sequence.Pay.Transak
{
    internal class TransakFiatPay : IFiatPay
    {
        private TransakOnRamp _onRamp;
        private TransakNFTCheckout _checkout;
        private Dictionary<Address, TransakContractId> _contractIds;

        public TransakFiatPay(TransakOnRamp onRamp, TransakNFTCheckout checkout)
        {
            _onRamp = onRamp;
            _checkout = checkout;

            _contractIds = new Dictionary<Address, TransakContractId>();
        }

        public async Task OnRamp()
        {
            _onRamp.OpenTransakLink();
        }

        public async Task<string> GetOnRampLink()
        {
            return _onRamp.GetTransakLink();
        }

        public Task<string> GetNftCheckoutLink(CollectibleOrder order, ulong amount, NFTType nftType = NFTType.ERC721,
            Address recipient = null, AdditionalFee additionalFee = null,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract)
        {
            return _checkout.GetNFTCheckoutLink(order, amount, nftType, additionalFee);
        }

        public void ConfigureSaleContractId(TransakContractId contractId)
        {
            if (_contractIds.TryGetValue(contractId.ContractAddress, out var id))
            {
                if (id == contractId)
                {
                    return;
                }
                Debug.LogWarning($"{nameof(_contractIds)} already contains a different {nameof(TransakContractId)} for {contractId.ContractAddress}. Replacing existing instance with provided...");
            }
            _contractIds[contractId.ContractAddress] = contractId;
        }

        public Task<string> GetNftCheckoutLink(ERC1155Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, byte[] data = null, FixedByte[] proof = null)
        {
            if (_contractIds.TryGetValue(saleContract, out var contractId))
            {
                return _checkout.GetNFTCheckoutLink(saleContract, collection, tokenId, (ulong)amount, contractId, data, proof);
            }
            
            throw new InvalidOperationException($"No contract ID found for provided {nameof(saleContract)} contract address; please call {nameof(ConfigureSaleContractId)} first");
        }

        public Task<string> GetNftCheckoutLink(ERC721Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, byte[] proof = null)
        {
            if (_contractIds.TryGetValue(saleContract, out var contractId))
            {
                return _checkout.GetNFTCheckoutLink(saleContract, collection, tokenId, (ulong)amount, contractId, proof);
            }
            
            throw new InvalidOperationException($"No contract ID found for provided {nameof(saleContract)} contract address; please call {nameof(ConfigureSaleContractId)} first");
        }

        public Task<string> GetNftCheckoutLink(CollectibleOrder[] orders, BigInteger quantity, NFTType nftType = NFTType.ERC721,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract, Address recipient = null,
            AdditionalFee[] additionalFee = null)
        {
            return _checkout.GetNFTCheckoutLink(orders, (ulong)quantity, nftType, additionalFee);
        }
    }
}