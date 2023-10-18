using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Vector2 = UnityEngine.Vector2;

namespace Sequence.Demo
{
    public class ContentFetcher : IContentFetcher
    {
        private Queue<TokenElement> _tokenQueue = new Queue<TokenElement>();
        private Queue<NftElement> _nftQueue = new Queue<NftElement>();
        private List<Chain> _includeChains;
        private List<IIndexer> _indexers;
        private Address _address;
        private bool _more = true;
        private bool _isFetching = false;

        public ContentFetcher(Address address, params Chain[] includeChains)
        {
            _address = address;
            
            _includeChains = includeChains.ConvertToList();

            _indexers = new List<IIndexer>();
            int chains = _includeChains.Count;
            for (int i = 0; i < chains; i++)
            {
                _indexers.Add(new ChainIndexer((int)_includeChains[i]));
            }
        }

        public event Action<FetchContentResult> OnContentFetch;
        public event Action<CollectionProcessingResult> OnCollectionProcessing;

        public async Task FetchContent(int pageSize)
        {
            _isFetching = true;
            int chainIndex = 0;
            int pageNumber = 0;
            int indexers = _indexers.Count;
            Debug.Log("Fetching content...");
            while (_more)
            {
                GetTokenBalancesReturn balances = await _indexers[chainIndex].GetTokenBalances(
                    new GetTokenBalancesArgs(
                        _address,
                        true,
                        new Page { page = pageNumber, pageSize = pageSize }));
                Page returnedPage = balances.page;
                if (returnedPage.more)
                {
                    pageNumber = returnedPage.page;
                }
                else
                {
                    Debug.Log("Moving to next chain...");
                    pageNumber = 0;
                    chainIndex++;
                    if (chainIndex >= indexers)
                    {
                        Debug.Log("No more chains to fetch from.");
                        _more = false;
                    }
                }
                OnContentFetch?.Invoke(new FetchContentResult(balances.balances, _more));
                await AddTokensToQueues(balances.balances, _indexers[chainIndex], pageSize);
            }
        }

        private async Task AddTokensToQueues(TokenBalance[] tokenBalances, IIndexer indexer, int pageSize)
        {
            int items = tokenBalances.Length;
            for (int i = 0; i < items; i++)
            {
                if (tokenBalances[i].IsToken())
                {
                    TokenElement token = await BuildTokenElement(tokenBalances[i]);
                    if (token != null)
                    {
                        _tokenQueue.Enqueue(token);
                    }
                }else if (tokenBalances[i].IsNft())
                {
                    await ProcessCollection(tokenBalances[i], indexer, pageSize);
                }
            }
        }

        private async Task<TokenElement> BuildTokenElement(TokenBalance tokenBalance)
        {
            Sprite tokenIconSprite = await FetchIconSprite(tokenBalance);

            ContractInfo contractInfo = tokenBalance.contractInfo;
            if (contractInfo == null)
            {
                Debug.LogWarning($"No contractInfo found for given token: {tokenBalance}");
                return null;
            }

            BigInteger balance = tokenBalance.balance / (BigInteger)Math.Pow(10, (int)contractInfo.decimals);

            return new TokenElement(tokenBalance.contractAddress, tokenIconSprite, contractInfo.name,
                (Chain)(int)contractInfo.chainId, (uint)balance, contractInfo.symbol, new MockCurrencyConverter()); // Todo replace MockCurrencyConverter with real implementation
        }

        private async Task ProcessCollection(TokenBalance tokenBalance, IIndexer indexer, int pageSize)
        {
            Debug.Log($"Processing collection: {tokenBalance}");
            bool more = true;
            int pageNumber = 0;
            while (more)
            {
                GetTokenBalancesReturn balances = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(
                        _address,
                        tokenBalance.contractAddress,
                        true,
                        new Page { page = pageNumber, pageSize = pageSize }));
                Page returnedPage = balances.page;
                if (returnedPage.more)
                {
                    pageNumber = returnedPage.page;
                }
                else
                {
                    more = false;
                }
                OnCollectionProcessing?.Invoke(new CollectionProcessingResult(balances.balances, _more));
                await AddNftsToQueue(balances.balances);
            }
        }

        private async Task AddNftsToQueue(TokenBalance[] tokenBalances)
        {
            Debug.Log("Adding nfts to queue");
            int items = tokenBalances.Length;
            for (int i = 0; i < items; i++)
            {
                if (tokenBalances[i].IsNft())
                {
                    NftElement nft = await BuildNftElement(tokenBalances[i]);
                    if (nft != null)
                    {
                        _nftQueue.Enqueue(nft);
                    }
                }
                else
                {
                    throw new ArgumentException("Only ERC721/ERC1155s should be provided to this method");
                }
            }
        }

        private async Task<NftElement> BuildNftElement(TokenBalance tokenBalance)
        {
            Debug.Log("Building nft element");
            Debug.Log("Fetching collection icon sprite");
            Sprite collectionIconSprite = await FetchIconSprite(tokenBalance);
            Debug.Log("Fetching nft icon sprite");
            Sprite nftIconSprite = await FetchNftImageSprite(tokenBalance);

            ContractInfo contractInfo = tokenBalance.contractInfo;
            if (contractInfo == null)
            {
                Debug.LogWarning($"No contractInfo found for given token: {tokenBalance}");
                return null;
            }

            TokenMetadata metadata = tokenBalance.tokenMetadata;
            if (metadata == null)
            {
                Debug.LogWarning($"No metadata found for given token: {tokenBalance}");
                return null;
            }

            return new NftElement(new Address(tokenBalance.contractAddress), nftIconSprite, metadata.name,
                collectionIconSprite, contractInfo.name, metadata.tokenId, (Chain)(int)contractInfo.chainId,
                (uint)tokenBalance.balance, 1, new MockCurrencyConverter()); // Todo replace MockCurrencyConverter with real implementation
            // Todo figure out ethValue
        }

        private async Task<Sprite> FetchIconSprite(TokenBalance tokenBalance)
        {
            string metadataUrl = "";
            ContractInfo contractInfo = tokenBalance.contractInfo;
            if (contractInfo != null && contractInfo.logoURI != null && contractInfo.logoURI.Length > 0)
            {
                metadataUrl = contractInfo.logoURI;
            }
            else
            {
                Debug.Log($"No metadata URL found for given token: {tokenBalance}");
            }
            
            Sprite iconSprite = await SpriteFetcher.Fetch(metadataUrl);
            return iconSprite;
        }

        private async Task<Sprite> FetchNftImageSprite(TokenBalance tokenBalance)
        {
            string metadataUrl = "";
            TokenMetadata metadata = tokenBalance.tokenMetadata;
            if (metadata != null && metadata.image != null && metadata.image.Length > 0)
            {
                metadataUrl = metadata.image;
            }
            else
            {
                Debug.Log($"No metadata URL found for given token: {tokenBalance}");
            }
            
            Sprite iconSprite = await SpriteFetcher.Fetch(metadataUrl);
            return iconSprite;
        }

        public async Task<FetchTokenContentResult> FetchTokenContent(int maxToFetch)
        {
            int tokensFetched = _tokenQueue.Count;
            while (tokensFetched < maxToFetch && _more)
            {
                if (!_isFetching)
                {
                    FetchContent(maxToFetch);
                }
                await Task.Yield();
                tokensFetched = _tokenQueue.Count;
            }
            TokenElement[] tokens = new TokenElement[maxToFetch];
            for (int i = 0; i < maxToFetch; i++)
            {
                tokens[i] = _tokenQueue.Dequeue();
            }

            return new FetchTokenContentResult(tokens, _more || _tokenQueue.Count > 0);
        }

        public async Task<FetchNftContentResult> FetchNftContent(int maxToFetch)
        {
            int nftsFetched = _nftQueue.Count;
            while (nftsFetched < maxToFetch && _more)
            {
                if (!_isFetching)
                {
                    FetchContent(maxToFetch);
                }
                await Task.Yield();
                nftsFetched = _nftQueue.Count;
            }
            NftElement[] nfts = new NftElement[maxToFetch];
            for (int i = 0; i < maxToFetch; i++)
            {
                nfts[i] = _nftQueue.Dequeue();
            }

            return new FetchNftContentResult(nfts, _more || _nftQueue.Count > 0);
        }
        
        public Address GetAddress()
        {
            return _address;
        }
    }
}