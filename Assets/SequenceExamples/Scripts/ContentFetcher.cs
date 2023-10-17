using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Demo
{
    public class ContentFetcher : IContentFetcher
    {
        private List<TokenBalance> _content = new List<TokenBalance>();
        private Queue<TokenElement> _tokenQueue = new Queue<TokenElement>();
        private Queue<NftElement> _nftQueue = new Queue<NftElement>();
        private List<Chain> _includeChains;
        private List<IIndexer> _indexers;
        private Address _address;
        private bool _more = true;

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

        public event Action<FetchContentResult> OnContentFetchSuccess;
        
        public async Task FetchContent(int pageSize)
        {
            int chainIndex = 0;
            int pageNumber = 0;
            int indexers = _indexers.Count;
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
                    pageNumber = 0;
                    chainIndex++;
                    if (chainIndex >= indexers)
                    {
                        _more = false;
                    }
                }
                OnContentFetchSuccess?.Invoke(new FetchContentResult(balances.balances, _more));
                await AddContentToLists(balances.balances);
            }
        }

        private async Task AddContentToLists(TokenBalance[] tokenBalances)
        {
            int items = tokenBalances.Length;
            for (int i = 0; i < items; i++)
            {
                if (tokenBalances[i].IsToken() || tokenBalances[i].IsNft())
                {
                    _content.Add(tokenBalances[i]);
                }
                
                if (tokenBalances[i].IsToken())
                {
                    TokenElement token = await BuildTokenElement(tokenBalances[i]);
                    _tokenQueue.Enqueue(token);
                }
                else if (tokenBalances[i].IsNft())
                {
                    NftElement nft = await BuildNftElement(tokenBalances[i]);
                    _nftQueue.Enqueue(nft);
                }
            }
        }

        private async Task<TokenElement> BuildTokenElement(TokenBalance tokenBalance)
        {
            Sprite tokenIconSprite = await FetchIconSprite(tokenBalance);

            ContractInfo contractInfo = tokenBalance.contractInfo;
            if (contractInfo == null)
            {
                Debug.LogError($"No contractInfo found for given token: {tokenBalance}");
            }

            return new TokenElement(tokenBalance.contractAddress, tokenIconSprite, contractInfo.name,
                (Chain)(int)contractInfo.chainId, (uint)tokenBalance.balance, contractInfo.symbol, new MockCurrencyConverter()); // Todo replace MockCurrencyConverter with real implementation
        }

        private async Task<NftElement> BuildNftElement(TokenBalance tokenBalance)
        {
            Sprite nftIconSprite = await FetchIconSprite(tokenBalance);

            ContractInfo contractInfo = tokenBalance.contractInfo;
            if (contractInfo == null)
            {
                Debug.LogError($"No contractInfo found for given token: {tokenBalance}");
            }

            return new NftElement(new Address(tokenBalance.contractAddress), nftIconSprite, contractInfo.name,
                nftIconSprite, contractInfo.name, (uint)tokenBalance.id, (Chain)(int)contractInfo.chainId,
                (uint)tokenBalance.balance, 1, new MockCurrencyConverter()); // Todo replace MockCurrencyConverter with real implementation
            // Todo figure out collection details
            // Todo figure out ethValue
        }

        private async Task<Sprite> FetchIconSprite(TokenBalance tokenBalance)
        {
            Texture2D logo = MockNftContentFetcher.CreateMockTexture(); // Default if no image metadata provided
            string metadataUrl = "";
            TokenMetadata metadata = tokenBalance.tokenMetadata;
            ContractInfo contractInfo = tokenBalance.contractInfo;
            if (metadata != null && metadata.image != null && metadata.image.Length > 0 &&
                !metadata.image.EndsWith("gif"))
            {
                metadataUrl = metadata.image;
            }else if (contractInfo != null && contractInfo.logoURI != null && contractInfo.logoURI.Length > 0)
            {
                metadataUrl = contractInfo.logoURI;
            }
            else
            {
                Debug.Log($"No metadata URL found for given token: {tokenBalance}");
            }

            if (metadataUrl.Length > 0)
            {
                using (var imageRequest = UnityWebRequestTexture.GetTexture(metadataUrl))
                {
                    await imageRequest.SendWebRequest();

                    if (imageRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"Error fetching metadata image for token balance: {tokenBalance}. Error: {imageRequest.error}");
                    }
                    else
                    {
                        logo = ((DownloadHandlerTexture) imageRequest.downloadHandler).texture;
                    }
                }
            }
            
            Sprite iconSprite = Sprite.Create(logo, new Rect(0, 0, logo.width, logo.height),
                new Vector2(.5f, .5f));
            return iconSprite;
        }

        public async Task<FetchTokenContentResult> FetchTokenContent(int maxToFetch)
        {
            int tokensFetched = _tokenQueue.Count;
            while (tokensFetched < maxToFetch && _more)
            {
                await Task.Yield();
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
                await Task.Yield();
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