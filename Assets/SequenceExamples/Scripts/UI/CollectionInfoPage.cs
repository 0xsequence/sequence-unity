using System;
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

        private NetworkIcons _networkIcons;
        private CollectionInfo _collectionInfo;
        private INftContentFetcher _nftContentFetcher;
        private ObjectPool _nftWithInfoTextObjectPool;
        private WalletPanel _walletPanel;
        private RectTransform _nftsWithInfoGridLayoutGroupTransform;

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
            _networkName.text = ChainNames.NameOf[_collectionInfo.Network];

            if (_walletPanel == null)
            {
                _walletPanel = FindObjectOfType<WalletPanel>();
            }
            List<NftElement> nftsInCollection = _walletPanel.GetNftsFromCollection(_collectionInfo);
            _uniqueCollectiblesOwnedText.text = $"{nftsInCollection.Count} Unique Collectibles";
            _totalOwnedText.text = $"Owned({CalculateTotalNftsOwned(nftsInCollection)})";
            PopulateNftUIElements(nftsInCollection);
        }

        private uint CalculateTotalNftsOwned(List<NftElement> nfts)
        {
            int count = nfts.Count;
            uint owned = 0;
            for (int i = 0; i < count; i++)
            {
                owned += nfts[i].Balance;
            }
            return owned;
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
                uiElement.Assemble(nfts[i]);
            }
        }
    }
}