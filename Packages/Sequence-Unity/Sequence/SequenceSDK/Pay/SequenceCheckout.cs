using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay.Sardine;
using Sequence.Pay.Transak;

namespace Sequence.Pay
{
    public class SequenceCheckout : IFiatCheckout
    {
        private SequencePay _pay;
        private CollectibleOrder[] _orders;
        private ERC1155Sale _erc1155Sale;
        private ERC721Sale _erc721Sale;
        private Address _collection;
        private Dictionary<string, BigInteger> _amountsByTokenId;
        private BigInteger _amount;
        private string _tokenId;
        private NFTType _nftType;
        private Address _recipient;
        private AdditionalFee[] _additionalFees;
        private byte[] _data;
        private FixedByte[] _proof;
        private string _marketplaceAddress;

        private enum Config
        {
            MARKETPLACE,
            ERC1155,
            ERC721
        }
        
        private Config _config;

        public SequenceCheckout(IWallet wallet, Chain chain, CollectibleOrder[] orders, BigInteger amount, NFTType nftType = NFTType.ERC1155, string marketplaceAddress = ISardineCheckout.SequenceMarketplaceV2Contract, Address recipient = null, AdditionalFee[] additionalFees = null, SequencePay pay = null)
        {
            if (pay == null)
            {
                pay = new SequencePay(wallet, chain);
            }
            
            if (recipient == null)
            {
                recipient = wallet.GetWalletAddress();
            }

            _recipient = recipient;

            _pay = pay;
            _orders = orders;
            _config = Config.MARKETPLACE;
            _amount = amount;
            _nftType = nftType;
            _additionalFees = additionalFees;
            _marketplaceAddress = marketplaceAddress;
        }
        
        public SequenceCheckout(IWallet wallet, Chain chain, ERC1155Sale saleContract, Address collection, Dictionary<string, BigInteger> amountsByTokenId, Address recipient = null, byte[] data = null, FixedByte[] proof = null, SequencePay pay = null)
        {
            if (pay == null)
            {
                pay = new SequencePay(wallet, chain);
            }
            
            if (recipient == null)
            {
                recipient = wallet.GetWalletAddress();
            }

            _recipient = recipient;

            _pay = pay;
            _erc1155Sale = saleContract;
            _collection = collection;
            _amountsByTokenId = amountsByTokenId;
            _config = Config.ERC1155;
            _data = data;
            _proof = proof;
        }
        
        public SequenceCheckout(IWallet wallet, Chain chain, ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount, Address recipient = null, FixedByte[] proof = null, SequencePay pay = null)
        {
            if (pay == null)
            {
                pay = new SequencePay(wallet, chain);
            }
            
            if (recipient == null)
            {
                recipient = wallet.GetWalletAddress();
            }

            _recipient = recipient;

            _pay = pay;
            _erc721Sale = saleContract;
            _collection = collection;
            _amount = amount;
            _tokenId = tokenId;
            _config = Config.ERC721;
            _proof = proof;
        }

        public bool OnRampEnabled()
        {
            return _pay.IsOnRampAvailable();
        }

        public Task<bool> NftCheckoutEnabled()
        {
            switch (_config)
            {
                case Config.MARKETPLACE:
                    return _pay.NftCheckoutEnabled(_orders);
                case Config.ERC1155:
                    return _pay.NftCheckoutEnabled(_erc1155Sale, _collection, _amountsByTokenId);
                case Config.ERC721:
                    return _pay.NftCheckoutEnabled(_erc721Sale, _collection, _tokenId, _amount);
                default:
                    throw new SystemException($"Encountered unknown configuration type: {_config}");
            }
        }

        public Task<string> GetOnRampLink()
        {
            return _pay.GetOnRampLink();
        }

        public Task<string> GetNftCheckoutLink()
        {
            switch (_config)
            {
                case Config.MARKETPLACE:
                    return _pay.GetNftCheckoutLink(_orders, _amount, _nftType, _marketplaceAddress, _recipient, _additionalFees);
                case Config.ERC1155:
                    return _pay.GetNftCheckoutLink(_erc1155Sale, _collection, BigInteger.Parse(_tokenId), _amount, _recipient, _data,
                        _proof);
                case Config.ERC721:
                    return _pay.GetNftCheckoutLink(_erc721Sale, _collection, BigInteger.Parse(_tokenId), _amount, _recipient, _proof);
                default:
                    throw new SystemException($"Encountered unknown configuration type: {_config}");
            }
        }
    }
}