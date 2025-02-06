using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Contracts;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Marketplace;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Utils;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.Integrations.Transak
{
    public class TransakNFTCheckout
    {
        private IWallet _wallet;
        private Chain _chain;
        private IEthClient _client;
        private ICheckout _checkout;
        private IMarketplaceReader _reader;
        private Address _walletAddress;

        // Referenced from here: https://docs.transak.com/docs/nft-marketplace
        // We are to provide the Chain-associated contract address as the "to" address during NFT checkout
        private static readonly Dictionary<Chain, Address> TransakContractAddresses = new Dictionary<Chain, Address>()
        {
            {Chain.Ethereum, new Address("0xab88cd272863b197b48762ea283f24a13f6586dd")},
            {Chain.TestnetSepolia, new Address("0xD84aC4716A082B1F7eCDe9301aA91A7c4B62ECd7")},
            {Chain.Polygon, new Address("0x4A598B7eC77b1562AD0dF7dc64a162695cE4c78A")},
            {Chain.TestnetPolygonAmoy, new Address("0xCB9bD5aCD627e8FcCf9EB8d4ba72AEb1Cd8Ff5EF")},
            {Chain.BNBSmartChain, new Address("0x4A598B7eC77b1562AD0dF7dc64a162695cE4c78A")},
            {Chain.TestnetBNBSmartChain, new Address("0x0E9539455944BE8a307bc43B0a046613a1aD6732")},
            {Chain.ArbitrumOne, new Address("0x4A598B7eC77b1562AD0dF7dc64a162695cE4c78A")},
            {Chain.TestnetArbitrumSepolia, new Address("0x489F56e3144FF03A887305839bBCD20FF767d3d1")},
            {Chain.Optimism, new Address("0x4A598B7eC77b1562AD0dF7dc64a162695cE4c78A")},
            {Chain.TestnetOptimisticSepolia, new Address("0xCB9bD5aCD627e8FcCf9EB8d4ba72AEb1Cd8Ff5EF")},
            {Chain.ImmutableZkEvm, new Address("0x8b83dE7B20059864C479640CC33426935DC5F85b")},
            {Chain.TestnetImmutableZkEvm, new Address("0x489F56e3144FF03A887305839bBCD20FF767d3d1")},
            {Chain.Base, new Address("0x8b83dE7B20059864C479640CC33426935DC5F85b")},
            {Chain.TestnetBaseSepolia, new Address("0xCB9bD5aCD627e8FcCf9EB8d4ba72AEb1Cd8Ff5EF")},
        };

        public TransakNFTCheckout(IWallet wallet, Chain chain, IEthClient client = null, ICheckout checkout = null, IMarketplaceReader reader = null)
        {
            if (!TransakContractAddresses.ContainsKey(chain))
            {
                throw new NotSupportedException($"The provided chain is not supported for Transak NFT checkout, given: {chain}. Supported chains include: {string.Join(", ",TransakContractAddresses.Keys)}");
            }
            
            _wallet = wallet;
            _chain = chain;
            if (client == null)
            {
                client = new SequenceEthClient(chain);
            }

            _client = client;

            _walletAddress = _wallet.GetWalletAddress();

            _checkout = checkout;
            if (_checkout == null)
            {
                _checkout = new Checkout(_wallet, chain);
            }

            if (reader == null)
            {
                reader = new MarketplaceReader(_chain);
            }

            _reader = reader;
        }

        public static Task<SupportedCountry[]> GetSupportedCountries()
        {
            return TransakOnRamp.GetSupportedCountries();
        }

        /// <summary>
        /// Get a link that, when opened, allows the user to buy the specified token with their credit card using Transak
        ///
        /// The provided Contract, buyFunctionName, and buyFunctionArgs are used to encode the calldata for the buy function that is executed
        /// on the token contract when purchasing the token. For more info, please see the Transak documentation:
        /// https://docs.transak.com/docs/query-params-for-marketplaces
        /// </summary>
        /// <param name="item" the token the user wants to purchase></param>
        /// <param name="contract" the contract id for Transak obtained by Sequence team></param>
        /// <param name="contract" the token contract></param>
        /// <param name="buyFunctionName" the name of buy function that needs to be executed on the token contract></param>
        /// <param name="buyFunctionArgs" the arguments needed for the buy function that needs to be executed on the token contract></param>
        /// <returns></returns>
        public async Task<string> GetNFTCheckoutLink(TransakNftData item, TransakContractId contractId, Contract contract, string buyFunctionName, params object[] buyFunctionArgs)
        {
            string callData = contract.AssembleCallData(buyFunctionName, buyFunctionArgs);

            return await GetNFTCheckoutLink(item, callData, contract.GetAddress(), contractId);
        }

        public async Task OpenNFTCheckoutLink(TransakNftData item, TransakContractId contractId, Contract contract, string buyFunctionName,
            params object[] buyFunctionArgs)
        {
            string link = await GetNFTCheckoutLink(item, contractId, contract, buyFunctionName, buyFunctionArgs);
            Application.OpenURL(link);
        }
        
        private async Task<string> GetNFTCheckoutLink(TransakNftData item, string callData, Address contractAddress, TransakContractId contractId)
        {
            if (contractId.Chain != _chain)
            {
                throw new ArgumentException($"The provided {nameof(contractId)} is not for the same chain as the current instance of {nameof(TransakNFTCheckout)}, given: {contractId.Chain}, expected: {_chain}");
            }

            string transakCallData = CallDataCompressor.Compress(callData);
            
            string baseUrl = "https://global.transak.com";
            string transakContractId = contractId.Id;
            
            string transakCryptocurrencyCode = contractId.PriceTokenSymbol;
            
            string transakNftDataEncoded = NftDataEncoder.Encode(item);
            
            // Todo use real gas estimates - issue right now is that the call to the Eth client is giving an 'execution reverted' error
            // Most likely reverting since they (probably) don't have any gas in the contract and are likely using a relayer of some sort
            
            // GasLimitEstimator gasLimitEstimator = new GasLimitEstimator(_client, TransakContractAddresses[_chain]);
            // BigInteger estimatedGasLimit =
            //     await gasLimitEstimator.EstimateGasLimit(contractAddress, callData, BigInteger.Zero);
            BigInteger estimatedGasLimit = 500000;
            string partnerOrderId = $"{_walletAddress}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            string transakLink = $"{baseUrl.AppendTrailingSlashIfNeeded()}?apiKey={SequenceTransakContractIdRepository.ApiKey}" +
                                 $"&isNFT=true" +
                                 $"&calldata={transakCallData}" +
                                 $"&contractId={transakContractId}" +
                                 $"&cryptoCurrencyCode={transakCryptocurrencyCode}" +
                                 $"&estimatedGasLimit={estimatedGasLimit}" +
                                 $"&nftData={transakNftDataEncoded}" +
                                 $"&walletAddress={_walletAddress}" +
                                 $"&disableWalletAddressForm=true" +
                                 $"&partnerOrderId={partnerOrderId}";

            return transakLink;
        }

        public async Task<string> GetNFTCheckoutLink(CollectibleOrder collectibleOrder, ulong quantity, TransakContractId contractId, NFTType nftType = NFTType.ERC721, AdditionalFee additionalFee = null)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException($"{nameof(quantity)} must be greater than 0");
            }

            Order order = collectibleOrder.order;
            TokenMetadata metadata = collectibleOrder.metadata;
            
            Step[] steps = await _checkout.GenerateBuyTransaction(order, quantity, additionalFee, TransakContractAddresses[_chain]);
            string callData = steps.ExtractBuyStep().data;

            TransakNftData nftData = new TransakNftData(metadata.image, metadata.name,
                new Address(order.collectionContractAddress), new string[] { order.tokenId }, 
                new decimal[] { DecimalNormalizer.ReturnToNormalPrecise(ulong.Parse(order.priceAmount), (int)order.priceDecimals) }, 
                quantity, nftType);
            
            return await GetNFTCheckoutLink(nftData, callData, new Address(order.collectionContractAddress), contractId);
        }
        
        public async Task OpenNFTCheckoutLink(CollectibleOrder order, ulong quantity, TransakContractId contractId, NFTType nftType = NFTType.ERC721)
        {
            string link = await GetNFTCheckoutLink(order, quantity, contractId, nftType);
            Application.OpenURL(link);
        }

        public async Task<string> GetNFTCheckoutLink(ERC1155Sale saleContract, Address collection, BigInteger tokenId,
            ulong quantity, TransakContractId contractId, byte[] data = null, FixedByte[] proof = null)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException($"{nameof(quantity)} must be greater than 0");
            }

            Address paymentToken = await saleContract.GetPaymentTokenAsync(_client);
            ERC1155Sale.SaleDetails saleDetails = await saleContract.TokenSaleDetailsAsync(_client, tokenId);
            TokenMetadata metadata = await _reader.GetCollectible(collection, tokenId.ToString());
            ERC20 paymentTokenContract = new ERC20(paymentToken);
            BigInteger decimals = await paymentTokenContract.Decimals(_client);

            string callData = saleContract.Mint(TransakContractAddresses[_chain], new[] { tokenId }, new[] { (BigInteger)quantity }, data, paymentToken,
                saleDetails.Cost * quantity, proof).CallData;
            
            TransakNftData nftData = new TransakNftData(metadata.image, metadata.name, collection, new []{tokenId.ToString()}, 
                new [] { DecimalNormalizer.ReturnToNormalPrecise(saleDetails.Cost, (int)decimals) }, quantity, NFTType.ERC1155);

            return await GetNFTCheckoutLink(nftData, callData, collection, contractId);
        }

        public async Task OpenNFTCheckoutLink(ERC1155Sale saleContract, Address collection, BigInteger tokenId,
            ulong quantity, TransakContractId contractId, byte[] data = null, FixedByte[] proof = null)
        {
            string link = await GetNFTCheckoutLink(saleContract, collection, tokenId, quantity, contractId, data, proof);
            Application.OpenURL(link);
        }

        public async Task<string> GetNFTCheckoutLink(ERC721Sale saleContract, Address collection, BigInteger tokenId,
            ulong quantity, TransakContractId contractId, byte[] proof = null)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException($"{nameof(quantity)} must be greater than 0");
            }

            ERC721Sale.SaleDetails saleDetails = await saleContract.GetSaleDetailsAsync(_client);
            Address paymentToken = saleDetails.PaymentToken;
            TokenMetadata metadata = await _reader.GetCollectible(collection, tokenId.ToString());
            ERC20 paymentTokenContract = new ERC20(paymentToken);
            BigInteger decimals = await paymentTokenContract.Decimals(_client);

            string callData = saleContract.Mint(TransakContractAddresses[_chain], quantity, paymentToken,
                saleDetails.Cost * quantity, proof).CallData;
            
            TransakNftData nftData = new TransakNftData(metadata.image, metadata.name, collection, new []{tokenId.ToString()}, 
                new [] { DecimalNormalizer.ReturnToNormalPrecise(saleDetails.Cost, (int)decimals) }, quantity, NFTType.ERC1155);

            return await GetNFTCheckoutLink(nftData, callData, collection, contractId);
        }
    }
}