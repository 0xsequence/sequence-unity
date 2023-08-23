using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.Demo
{
    public class WalletPage : UIPage
    {
        [SerializeField] private int numberOfNftPlaceholdersToInstantiate = 10;
        [SerializeField] private GameObject NftPlaceHolderPrefab;
        [SerializeField] private int numberOfNftsToFetchAtOnce = 1;
        private ObjectPool _pool;
        private INftContentFetcher _nftFetcher;
        private List<Texture2D> _nftContent;

        protected override void Awake()
        {
            base.Awake();
            _pool = ObjectPool.ActivateObjectPool(NftPlaceHolderPrefab, numberOfNftPlaceholdersToInstantiate);
        }

        public override void Open(params object[] args)
        {
            base.Open();

            if (_nftFetcher == null)
            {
                throw new SystemException($"{nameof(_nftFetcher)} must not be null. Please call {nameof(SetupContentFetchers)} before opening");
            }

            _nftFetcher.FetchContent(numberOfNftsToFetchAtOnce);
        }

        public override void Close()
        {
            base.Close();
            _nftFetcher = null;
        }

        public void SetupContentFetchers(INftContentFetcher nftContentFetcher)
        {
            _nftFetcher = nftContentFetcher;
            _nftFetcher.OnNftFetchSuccess += HandleNftFetchSuccess;
        }

        private void HandleNftFetchSuccess(FetchNftContentResult result)
        {
            Texture2D[] textures = result.Content;
            int count = textures.Length;
            for (int i = 0; i < count; i++)
            {
                _nftContent.Add(textures[i]);
            }

            if (result.MoreToFetch)
            {
                _nftFetcher.FetchContent(numberOfNftsToFetchAtOnce);
            }
        }
    }
}