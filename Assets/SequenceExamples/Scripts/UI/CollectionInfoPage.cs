using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class CollectionInfoPage : UIPage
    {
        [SerializeField] private Image _collectionIcon;
        [SerializeField] private TextMeshProUGUI _collectionNameText;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _networkName;
        [SerializeField] private TextMeshProUGUI _uniqueCollectiblesOwnedText;
        [SerializeField] private TextMeshProUGUI _totalOwnedText;
        [SerializeField] private GameObject _nftWithInfoTextPrefab;
        [SerializeField] private int _numberOfNftWithInfoTextPlaceholdersToInstantiate = 1;
        [SerializeField] private GridLayoutGroup _nftsWithInfoGridLayoutGroup;
        [SerializeField] private RectTransform _scrollRectContentTransform;

        private NetworkIcons _networkIcons;
        private CollectionInfo _collectionInfo;
        private INftContentFetcher _nftContentFetcher;
        private ObjectPool _nftWithInfoTextObjectPool;
        private WalletPanel _walletPanel;
        private RectTransform _nftsWithInfoGridLayoutGroupTransform;
        private float _totalOwnedTextBuffer = 15f;

        public override void Open(params object[] args)
        {
            base.Open(args);
            NetworkIcons networkIcons = args.GetObjectOfTypeIfExists<NetworkIcons>();
            if (networkIcons == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(NetworkIcons)} as an argument");
            }
            CollectionInfo collectionInfo = args.GetObjectOfTypeIfExists<CollectionInfo>();
            if (collectionInfo == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(CollectionInfo)} as an argument");
            }

            _networkIcons = networkIcons;
            _collectionInfo = collectionInfo;

            _nftWithInfoTextObjectPool = ObjectPool.ActivateObjectPool(_nftWithInfoTextPrefab,
                _numberOfNftWithInfoTextPlaceholdersToInstantiate);

            _nftsWithInfoGridLayoutGroupTransform = _nftsWithInfoGridLayoutGroup.GetComponent<RectTransform>();

            _walletPanel = (WalletPanel)_panel;

            Assemble();
        }

        public override void Close()
        {
            base.Close();
            _nftWithInfoTextObjectPool.Cleanup();
        }

        private void Assemble()
        {
            _collectionIcon.sprite = _collectionInfo.IconSprite;
            _collectionNameText.text = _collectionInfo.Name;
            _networkIcon.sprite = _networkIcons.GetIcon(_collectionInfo.Network);
            _networkName.text = ChainDictionaries.NameOf[_collectionInfo.Network];
            
            List<NftElement> nftsInCollection = _walletPanel.GetNftsFromCollection(_collectionInfo);
            _uniqueCollectiblesOwnedText.text = $"{nftsInCollection.Count} Unique Collectibles";
            _totalOwnedText.text = $"Owned ({NftElement.CalculateTotalNftsOwned(nftsInCollection)})";
            PopulateNftUIElements(nftsInCollection);
            StartCoroutine(UpdateScrollViewSize());
        }

        private void PopulateNftUIElements(List<NftElement> nfts)
        {
            int count = nfts.Count;
            for (int i = 0; i < count; i++)
            {
                Transform nftWithTextTransform = _nftWithInfoTextObjectPool.GetNextAvailable();
                if (nftWithTextTransform == null)
                {
                    throw new SystemException(
                        $"{nameof(nftWithTextTransform)} should not be null. {nameof(_nftWithInfoTextObjectPool)} should expand.");
                }
                
                nftWithTextTransform.SetParent(_nftsWithInfoGridLayoutGroupTransform);
                nftWithTextTransform.localScale = new Vector3(1, 1, 1);
                NftWithInfoTextUIElement uiElement = nftWithTextTransform.GetComponent<NftWithInfoTextUIElement>();
                uiElement.Assemble(nfts[i], _walletPanel);
            }
        }
        
        private IEnumerator UpdateScrollViewSize()
        {
            yield return new WaitForSeconds(_openAnimationDurationInSeconds);
            
            int itemCount = _nftsWithInfoGridLayoutGroupTransform.childCount;
            int rowCount = Mathf.CeilToInt((float)itemCount / _nftsWithInfoGridLayoutGroup.constraintCount);
            float contentHeight = rowCount * _nftsWithInfoGridLayoutGroup.cellSize.y + (rowCount - 1) * _nftsWithInfoGridLayoutGroup.spacing.y;

            RectTransform content = _scrollRectContentTransform;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight + _totalOwnedTextBuffer + _nftsWithInfoGridLayoutGroup.padding.top);
        }
    }
}