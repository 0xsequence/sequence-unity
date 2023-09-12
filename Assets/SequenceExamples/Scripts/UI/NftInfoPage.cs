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
    public class NftInfoPage : UIPage
    {
        [SerializeField] private Image _collectionIcon;
        [SerializeField] private TextExtender _collectionNameText;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _nftNameText;
        [SerializeField] private TextMeshProUGUI _nftNumberText;
        [SerializeField] private Image _nftImage;
        [SerializeField] private TextExtender _ethValueText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private GameObject _transactionDetailsBlockPlaceholderPrefab;
        [SerializeField] private int _numberOfTransactionDetailsBlockPlaceholdersToInstantiate = 1;
        [SerializeField] private int _numberOfTransactionDetailsToFetchAtOnce = 1;
        [SerializeField] private VerticalLayoutGroup _transactionDetailsBlockLayoutGroup;
        public float TimeBetweenEthValueRefreshesInSeconds = 5;
        private NetworkIcons _networkIcons;

        private NftElement _nftElement;
        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;

        private ObjectPool _transactionPool;
        private ITransactionDetailsFetcher _transactionDetailsFetcher;
        private List<TransactionDetails> _transactionDetails = new List<TransactionDetails>();
        private List<TransactionDetailsBlock> _transactionDetailsBlocks = new List<TransactionDetailsBlock>();
        
        private RectTransform _scrollRectContent;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private RectTransform _transactionDetailsBlockLayoutGroupRectTransform;
        
        protected override void Awake()
        {
            base.Awake();
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _verticalLayoutGroup = _scrollRectContent.GetComponent<VerticalLayoutGroup>();
            _transactionDetailsBlockLayoutGroupRectTransform =
                _transactionDetailsBlockLayoutGroup.GetComponent<RectTransform>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            NftElement nftElement = args.GetObjectOfTypeIfExists<NftElement>();
            if (nftElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(NftInfoPage)} must be opened with a {typeof(NftElement)} as an argument");
            }
            NetworkIcons networkIcons = args.GetObjectOfTypeIfExists<NetworkIcons>();
            if (networkIcons == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(NftInfoPage)} must be opened with a {typeof(NetworkIcons)} as an argument");
            }

            ITransactionDetailsFetcher transactionDetailsFetcher =
                args.GetObjectOfTypeIfExists<ITransactionDetailsFetcher>();
            if (transactionDetailsFetcher == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(NftInfoPage)} must be opened with a {typeof(ITransactionDetailsFetcher)} as an argument");
            }

            _nftElement = nftElement;
            _networkIcons = networkIcons;

            _transactionPool = ObjectPool.ActivateObjectPool(_transactionDetailsBlockPlaceholderPrefab,
                _numberOfTransactionDetailsBlockPlaceholdersToInstantiate);
            
            _transactionDetailsFetcher = transactionDetailsFetcher;
            _transactionDetailsFetcher.OnTransactionDetailsFetchSuccess += HandleTransactionDetailsFetchSuccess;
            _transactionDetailsFetcher.Refresh();
            _transactionDetailsFetcher.FetchTransactionsFromContract(_nftElement.ContractAddress, _numberOfTransactionDetailsToFetchAtOnce);
            
            UpdateScrollViewSize();
            Assemble();
            StartCoroutine(RefreshEthValueRepeatedly());
        }

        public override void Close()
        {
            base.Close();
            _transactionDetailsFetcher.OnTransactionDetailsFetchSuccess -= HandleTransactionDetailsFetchSuccess;
            _transactionDetailsFetcher = null;
            _transactionPool.Cleanup();
            _transactionDetails = new List<TransactionDetails>();
            _transactionDetailsBlocks = new List<TransactionDetailsBlock>();
        }

        private void Assemble()
        {
            _collectionIcon.sprite = _nftElement.CollectionIconSprite;
            _collectionNameText.SetText(_nftElement.CollectionName, true);
            _networkIcon.sprite = _networkIcons.GetIcon(_nftElement.Network);
            _nftNameText.text = _nftElement.TokenName;
            _nftNumberText.text = $"#{_nftElement.TokenNumber}";
            _nftImage.sprite = _nftElement.TokenIconSprite;
            
            _amountAndCurrencyTextSetter = new AmountAndCurrencyTextSetter(_ethValueText, _currencyValueText, _nftElement);
            _amountAndCurrencyTextSetter.SetInitialValueAndAmountText();
        }

        private IEnumerator RefreshEthValueRepeatedly()
        {
            var waitForRefresh = new WaitForSecondsRealtime(TimeBetweenEthValueRefreshesInSeconds);
            while (true) // Terminates on Close() (as this gameObject will be disabled)
            {
                yield return waitForRefresh;
                RefreshCurrencyValue();
            }
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshCurrencyValue();
        }
        
        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshWithAmount(balance);
        }

        private void ThrowIfNotAssembled()
        {
            if (_amountAndCurrencyTextSetter == null)
            {
                throw new SystemException(
                    $"{typeof(NftInfoPage)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        public Sprite GetNetworkIcon(Chain network)
        {
            return _networkIcons.GetIcon(network);
        }
        
        private void HandleTransactionDetailsFetchSuccess(FetchTransactionDetailsResult result)
        {
            TransactionDetails[] elements = result.Content;
            int count = elements.Length;
            for (int i = 0; i < count; i++)
            {
                _transactionDetails.Add(elements[i]);
                CreateTransactionDetailsBlock();
                UpdateScrollViewSize();
                SortTransactionDetails();
                PopulateTransactionDetailsBlocks();
            }

            if (result.MoreToFetch)
            {
                _transactionDetailsFetcher.FetchTransactionsFromContract(_nftElement.ContractAddress, _numberOfTransactionDetailsToFetchAtOnce);
            }
        }

        private void CreateTransactionDetailsBlock()
        {
            Transform transactionContainer = _transactionPool.GetNextAvailable();
            if (transactionContainer == null)
            {
                throw new SystemException(
                    $"{nameof(transactionContainer)} should not be null. {nameof(_transactionPool)} should expand.");
            }

            transactionContainer.SetParent(_transactionDetailsBlockLayoutGroupRectTransform);
            transactionContainer.localScale = new Vector3(1, 1, 1);
            TransactionDetailsBlock uiElement = transactionContainer.GetComponent<TransactionDetailsBlock>();
            _transactionDetailsBlocks.Add(uiElement);
        }

        private void UpdateScrollViewSize()
        {
            float contentHeight = _verticalLayoutGroup.preferredHeight;
            _scrollRectContent.sizeDelta = new Vector2(_scrollRectContent.sizeDelta.x, contentHeight);

            contentHeight = _transactionDetailsBlockLayoutGroup.preferredHeight;
            _transactionDetailsBlockLayoutGroupRectTransform.sizeDelta =
                new Vector2(_transactionDetailsBlockLayoutGroupRectTransform.sizeDelta.x, contentHeight);
        }

        private void SortTransactionDetails()
        {
            int count = _transactionDetails.Count;

            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    DateTime date1, date2;

                    if (DateTime.TryParse(_transactionDetails[j].Date, out date1) &&
                        DateTime.TryParse(_transactionDetails[j + 1].Date, out date2))
                    {
                        if (date1 < date2)
                        {
                            TransactionDetails temp = _transactionDetails[j];
                            _transactionDetails[j] = _transactionDetails[j + 1];
                            _transactionDetails[j + 1] = temp;
                        }
                    }
                    else
                    {
                        throw new SystemException($"{nameof(_transactionDetails)} contains an invalid date");
                    }
                }
            }
        }

        private void PopulateTransactionDetailsBlocks()
        {
            int length = _transactionDetailsBlocks.Count;
            for (int i = 0; i < length; i++)
            {
                _transactionDetailsBlocks[i].Assemble(_transactionDetails[i], _networkIcons);
            }
        }
    }
}