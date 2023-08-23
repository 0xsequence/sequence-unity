using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class WalletPage : UIPage
    {
        [SerializeField] private int numberOfNftPlaceholdersToInstantiate = 10;
        [SerializeField] private GameObject NftPlaceHolderPrefab;
        [SerializeField] private int numberOfNftsToFetchAtOnce = 1;
        [SerializeField] private Transform _scrollviewContentParent;
        private ObjectPool _pool;
        private INftContentFetcher _nftFetcher;
        private List<Texture2D> _nftContent = new List<Texture2D>();
        private ScrollRect _scrollRect;

        protected override void Awake()
        {
            base.Awake();
            _pool = ObjectPool.ActivateObjectPool(NftPlaceHolderPrefab, numberOfNftPlaceholdersToInstantiate);
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(OnScroll);
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
                Transform nftContainer = _pool.GetNextAvailable();
                if (nftContainer != null)
                {
                    Image nftImage = nftContainer.GetComponent<Image>();
                    nftImage.sprite = Sprite.Create(textures[i], new Rect(0, 0, textures[i].width, textures[i].height),
                        new Vector2(.5f, .5f));
                    nftContainer.parent = _scrollviewContentParent;
                    nftContainer.localScale = new Vector3(1, 1, 1);
                }
            }

            if (result.MoreToFetch)
            {
                _nftFetcher.FetchContent(numberOfNftsToFetchAtOnce);
            }
        }

        private void OnScroll(Vector2 scrollPosition)
        {
            Debug.Log($"Scrolling to x: {scrollPosition.x} y: {scrollPosition.y}");
        }
    }
}