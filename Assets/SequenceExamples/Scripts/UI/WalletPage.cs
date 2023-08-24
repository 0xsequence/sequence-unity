using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class WalletPage : UIPage
    {
        [SerializeField] private int _numberOfNftPlaceholdersToInstantiate = 10;
        [SerializeField] private GameObject _nftPlaceHolderPrefab;
        [SerializeField] private int _numberOfNftsToFetchAtOnce = 1;
        [SerializeField] private Transform _scrollviewContentParent;
        private ObjectPool _pool;
        private INftContentFetcher _nftFetcher;
        private List<Texture2D> _nftContent = new List<Texture2D>();
        private RectTransform _scrollRectContent;
        private int _widthInItems = 2;
        private GridLayoutGroup _grid;
        private float _brandingBuffer = 60;

        protected override void Awake()
        {
            base.Awake();
            _pool = ObjectPool.ActivateObjectPool(_nftPlaceHolderPrefab, _numberOfNftPlaceholdersToInstantiate);
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _grid = GetComponentInChildren<GridLayoutGroup>();
        }

        public override void Open(params object[] args)
        {
            base.Open();

            if (_nftFetcher == null)
            {
                throw new SystemException($"{nameof(_nftFetcher)} must not be null. Please call {nameof(SetupContentFetchers)} before opening");
            }

            _nftFetcher.FetchContent(_numberOfNftsToFetchAtOnce);
        }

        public override void Close()
        {
            base.Close();
            _nftFetcher.OnNftFetchSuccess -= HandleNftFetchSuccess;
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
                ApplyTexture(textures[i]);
                UpdateScrollViewSize();
            }

            if (result.MoreToFetch)
            {
                _nftFetcher.FetchContent(_numberOfNftsToFetchAtOnce);
            }
        }

        private void ApplyTexture(Texture2D texture)
        {
            Transform nftContainer = _pool.GetNextAvailable();
            if (nftContainer != null)
            {
                Image nftImage = nftContainer.GetComponent<Image>();
                nftImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(.5f, .5f));
                nftContainer.parent = _scrollviewContentParent;
                nftContainer.localScale = new Vector3(1, 1, 1);
            }
        }

        private void UpdateScrollViewSize()
        {
            int itemCount = _nftContent.Count;
            int rowCount = Mathf.CeilToInt((float)itemCount / _widthInItems);
            float contentHeight = rowCount * _grid.cellSize.y + (rowCount - 1) * _grid.spacing.y;

            RectTransform content = _scrollRectContent;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight + _brandingBuffer);
        }
    }
}