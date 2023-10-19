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
        private Queue<TokenBalance>[] _collectionsToProcess;

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

            _collectionsToProcess = new Queue<TokenBalance>[chains];
            for (int i = 0; i < chains; i++)
            {
                _collectionsToProcess[i] = new Queue<TokenBalance>();
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
                GetTokenBalancesArgs args = new GetTokenBalancesArgs(
                    _address,
                    true,
                    new Page { page = pageNumber, pageSize = pageSize });
                GetTokenBalancesReturn balances = await _indexers[chainIndex].GetTokenBalances(args);
                if (balances == null)
                {
                    Debug.LogWarning(
                        $"Received an error from indexer when fetching token balances with args: {args}\nCheck chain status here: https://status.sequence.info/\nMoving to next chain... {(Chain)(int)_indexers[chainIndex + 1].GetChainID()}");
                    
                    // ProcessCollectionsFromChain(chainIndex, pageSize);
                    chainIndex++;
                    pageNumber = 0;
                    if (chainIndex >= indexers)
                    {
                        Debug.Log("No more chains to fetch from.");
                        _more = false;
                    }

                    continue;
                }
                Page returnedPage = balances.page;
                if (returnedPage.more)
                {
                    pageNumber = returnedPage.page;
                    OnContentFetch?.Invoke(new FetchContentResult(balances.balances, _more));
                    AddTokensToQueues(balances.balances, chainIndex);
                }
                else
                {
                    // ProcessCollectionsFromChain(chainIndex, pageSize);
                    pageNumber = 0;
                    OnContentFetch?.Invoke(new FetchContentResult(balances.balances, _more));
                    AddTokensToQueues(balances.balances, chainIndex);
                    chainIndex++;
                    if (chainIndex >= indexers)
                    {
                        Debug.Log("No more chains to fetch from.");
                        _more = false;
                    }
                    else
                    {
                        Debug.Log($"Moving to next chain... {(Chain)(int)_indexers[chainIndex].GetChainID()}");
                    }
                }
            }

            for (int i = 0; i < indexers; i++)
            {
                await ProcessCollectionsFromChain(i, pageSize);
            }
        }

        private async Task AddTokensToQueues(TokenBalance[] tokenBalances, int indexerIndex)
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
                    _collectionsToProcess[indexerIndex].Enqueue(tokenBalances[i]);
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

            try
            {
                return new TokenElement(tokenBalance.contractAddress, tokenIconSprite, contractInfo.name,
                    (Chain)(int)contractInfo.chainId, (uint)balance, contractInfo.symbol,
                    new MockCurrencyConverter()); // Todo replace MockCurrencyConverter with real implementation
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to build token element for token: {tokenBalance}\nError: {e.Message}");
                return null;
            }
        }

        private async Task ProcessCollectionsFromChain(int chainIndex, int pageSize)
        {
            Queue<TokenBalance> toProcess = _collectionsToProcess[chainIndex];
            while (toProcess.TryDequeue(out TokenBalance tokenBalance))
            {
                Debug.Log($"Processing collections from {(Chain)(int)_indexers[chainIndex].GetChainID()}. Collections to process: {_collectionsToProcess[chainIndex].Count}");
                await ProcessCollection(tokenBalance, _indexers[chainIndex], pageSize);
            }
        }

        private async Task ProcessCollection(TokenBalance tokenBalance, IIndexer indexer, int pageSize)
        {
            bool more = true;
            int pageNumber = 0;
            int nftsFound = 0;
            while (more)
            {
                GetTokenBalancesReturn balances = await indexer.GetTokenBalances(
                    new GetTokenBalancesArgs(
                        _address,
                        tokenBalance.contractAddress,
                        true,
                        new Page { page = pageNumber, pageSize = pageSize }));
                if (balances == null)
                {
                    Debug.LogError($"Failed to finish processing collection: {tokenBalance}");
                    break;
                }
                Page returnedPage = balances.page;
                if (returnedPage.more)
                {
                    pageNumber = returnedPage.page;
                }
                else
                {
                    more = false;
                }

                nftsFound += balances.balances.Length;
                Debug.Log("Total NFTs found: " + nftsFound + " Balance according to tokenBalance: " + tokenBalance.balance);
                
                OnCollectionProcessing?.Invoke(new CollectionProcessingResult(balances.balances, more));
                AddNftsToQueue(balances.balances);
            }
        }

        private async Task AddNftsToQueue(TokenBalance[] tokenBalances)
        {
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
                    Debug.LogError($"Only ERC721/ERC1155s should be provided to this method! Given {tokenBalances[i]}");
                }
            }
        }

        private async Task<NftElement> BuildNftElement(TokenBalance tokenBalance)
        {
            Sprite collectionIconSprite = await FetchIconSprite(tokenBalance);
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

            try
            {
                BigInteger balance = tokenBalance.balance;
                if (contractInfo.decimals != 0)
                {
                    balance = tokenBalance.balance / (BigInteger)Math.Pow(10, (int)contractInfo.decimals);
                }
                return new NftElement(new Address(tokenBalance.contractAddress), nftIconSprite, metadata.name,
                    collectionIconSprite, contractInfo.name, metadata.tokenId, (Chain)(int)contractInfo.chainId,
                    (uint)balance, 1,
                    new MockCurrencyConverter()); // Todo replace MockCurrencyConverter with real implementation
                // Todo figure out ethValue
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to build NFT element for token: {tokenBalance}\nError: {e.Message}");
                return null;
            }
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
                Debug.LogWarning($"No metadata URL found for given token: {tokenBalance}");
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

            TokenElement[] tokens = new TokenElement[tokensFetched];
            for (int i = 0; i < tokensFetched; i++)
            {
                tokens[i] = _tokenQueue.Dequeue();
            }

            return new FetchTokenContentResult(tokens, _more || _tokenQueue.Count > 0);
        }

        public async Task<FetchNftContentResult> FetchNftContent(int maxToFetch)
        {
            int nftsFetched = _nftQueue.Count;
            while (nftsFetched < maxToFetch && (_more || CollectionsLeftToProcess()))
            {
                if (!_isFetching)
                {
                    FetchContent(maxToFetch);
                }
                await Task.Yield();
                nftsFetched = _nftQueue.Count;
            }
            NftElement[] nfts = new NftElement[nftsFetched];
            for (int i = 0; i < nftsFetched; i++)
            {
                nfts[i] = _nftQueue.Dequeue();
            }
            
            Debug.Log($"@ more {_more} | nftQueue length {_nftQueue.Count} | collections left to process {CollectionsLeftToProcess()}");

            return new FetchNftContentResult(nfts, _more || _nftQueue.Count > 0 || CollectionsLeftToProcess());
        }
        
        public Address GetAddress()
        {
            return _address;
        }

        private bool CollectionsLeftToProcess()
        {
            int chains = _collectionsToProcess.Length;
            for (int i = 0; i < chains; i++)
            {
                if (_collectionsToProcess[i].Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}