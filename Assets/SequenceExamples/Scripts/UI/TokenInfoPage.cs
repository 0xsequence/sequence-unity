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
    public class TokenInfoPage : UIPage
    {
        [SerializeField] private Image _tokenIconImage;
        [SerializeField] private TextMeshProUGUI _tokenNameText;
        [SerializeField] private Image _networkIconImage;
        [SerializeField] private TextMeshProUGUI _networkNameText;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private GameObject _transactionDetailsBlockPlaceholderPrefab;
        [SerializeField] private int _numberOfTransactionDetailsBlockPlaceholdersToInstantiate = 1;
        [SerializeField] private int _numberOfTransactionDetailsToFetchAtOnce = 1;
        public float TimeBetweenTokenValueRefreshesInSeconds = 5;
        private NetworkIcons _networkIcons;
        
        private TokenElement _tokenElement;
        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;

        private ObjectPool _transactionPool;
        private ITransactionDetailsFetcher _transactionDetailsFetcher;
        private List<TransactionDetails> _transactionDetails = new List<TransactionDetails>();

        private RectTransform _scrollRectContent;
        private VerticalLayoutGroup _verticalLayoutGroup;

        protected override void Awake()
        {
            base.Awake();
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _verticalLayoutGroup = _scrollRectContent.GetComponentInChildren<VerticalLayoutGroup>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            TokenElement tokenElement = args.GetObjectOfTypeIfExists<TokenElement>();
            if (tokenElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(TokenInfoPage)} must be opened with a {typeof(TokenElement)} as an argument");
            }
            NetworkIcons networkIcons = args.GetObjectOfTypeIfExists<NetworkIcons>();
            if (tokenElement == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(TokenInfoPage)} must be opened with a {typeof(NetworkIcons)} as an argument");
            }

            ITransactionDetailsFetcher transactionDetailsFetcher =
                args.GetObjectOfTypeIfExists<ITransactionDetailsFetcher>();
            if (transactionDetailsFetcher == default)
            {
                throw new SystemException(
                    $"Invalid use. {nameof(TokenInfoPage)} must be opened with a {typeof(ITransactionDetailsFetcher)} as an argument");
            }

            _tokenElement = tokenElement;
            _networkIcons = networkIcons;

            _transactionPool = ObjectPool.ActivateObjectPool(_transactionDetailsBlockPlaceholderPrefab,
                _numberOfTransactionDetailsBlockPlaceholdersToInstantiate);
            
            _transactionDetailsFetcher = transactionDetailsFetcher;
            _transactionDetailsFetcher.OnTransactionDetailsFetchSuccess += HandleTransactionDetailsFetchSuccess;
            _transactionDetailsFetcher.Refresh();
            _transactionDetailsFetcher.FetchTransactionsFromContract(_tokenElement.Erc20.GetAddress(), _numberOfTransactionDetailsToFetchAtOnce);
            
            Assemble();
            StartCoroutine(RefreshTokenValueRepeatedly());
        }

        public override void Close()
        {
            base.Close();
            _transactionDetailsFetcher.OnTransactionDetailsFetchSuccess -= HandleTransactionDetailsFetchSuccess;
            _transactionDetailsFetcher = null;
            _transactionPool.Cleanup();
            _transactionDetails = new List<TransactionDetails>();
        }

        private void Assemble()
        {
            _tokenIconImage.sprite = _tokenElement.TokenIconSprite;
            _tokenNameText.text = _tokenElement.TokenName;
            _networkIconImage.sprite = _networkIcons.GetIcon(_tokenElement.Network);
            _networkNameText.text = ChainNames.NameOf[_tokenElement.Network];

            _amountAndCurrencyTextSetter = new AmountAndCurrencyTextSetter(_balanceText, _currencyValueText, _tokenElement);
            _amountAndCurrencyTextSetter.SetInitialValueAndAmountText();
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
                    $"{typeof(TokenInfoPage)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        private IEnumerator RefreshTokenValueRepeatedly()
        {
            var waitForRefresh = new WaitForSecondsRealtime(TimeBetweenTokenValueRefreshesInSeconds);
            while (true) // Terminates on Close() (as this gameObject will be disabled)
            {
                yield return waitForRefresh;
                RefreshCurrencyValue();
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
                ApplyTransactionDetailsContent(elements[i]);
                UpdateScrollViewSize();
            }

            if (result.MoreToFetch)
            {
                _transactionDetailsFetcher.FetchTransactionsFromContract(_tokenElement.Erc20.GetAddress(), _numberOfTransactionDetailsToFetchAtOnce);
            }
        }

        private void ApplyTransactionDetailsContent(TransactionDetails details)
        {
            Transform transactionContainer = _transactionPool.GetNextAvailable();
            if (transactionContainer == null)
            {
                throw new SystemException(
                    $"{nameof(transactionContainer)} should not be null. {nameof(_transactionPool)} should expand.");
            }

            TransactionDetailsBlock uiElement = transactionContainer.GetComponent<TransactionDetailsBlock>();
            uiElement.Assemble(details, _networkIcons);
            transactionContainer.SetParent(_scrollRectContent);
            transactionContainer.localScale = new Vector3(1, 1, 1);
        }

        private void UpdateScrollViewSize()
        {
            float contentHeight = _verticalLayoutGroup.preferredHeight;

            RectTransform content = _scrollRectContent;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
        }
    }
}